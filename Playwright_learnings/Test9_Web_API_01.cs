using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Playwright_learnings
{
    /*
        - API-assisted UI testing is a testing approach where:
            * APIs are used to prepare test data, authenticate users, or perform backend operations.
            * UI automation is then used to verify the actual frontend functionality.
            
            * Instead of performing every step through UI, some operations are performed directly using APIs to make tests faster and more reliable.
        
        - Use case:
            * Consider testing of any shopping page. 
                As per traditional flow, if there are 10 test cases to test the page's features,
                It would be required to test/verify login scenario to test any inside feature.

                But using API-Assisted UI testing approach, we can use 1 dedicated test to veify login feature and later on we can:
                                - Send Login API Request
                                - Receive Token/Cookies
                                - Inject Authentication into Browser
                                - Open Application already logged in
                                - Perform UI testing for other features
            * Since goal is to achieve:
                                - Faster test execution
                                - Reducing flakiness due to slow loading, network delays
                                - Focused testing (focusing on targeted feature rather than login)
            
            * Another example: Test case is for verifying order showing up in history page.
                                - If there's an API to create order, we can use that to create order and use that order ID as our test data.
                                - Authorization can be fetched by Request Headers or depends on developement style.
    */

    [TestFixture]
    public class Test9_Web_API_01: CloudBrowserPageTest
    {
        // It is a class-level variable and can be accessed thoroughout class and can be shared between methods.
        private string token;


        [OneTimeSetUp]
        public async Task OTS()
        {
            var loginPayload = new  // This creates an anonymous object which is equivalent to json format and will be sent in POST request body.
            {                       // Actual datatype is Anonymous Type
                userEmail = "prateik.mallik@outlook.com",
                userPassword = "Dukekumar@96"
            };

            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var apiContext = await playwright.APIRequest.NewContextAsync();
            /*
                - It creates API request context.                               
                - It can be used to perform CRUD operations.
            

                - PostAsync() is used to send HTTP POST request.
                - Here, API endoint URL and request body is being passed.
                - loginResponse stores API response.
                - It's datatype is IAPIResponse
            */
            IAPIResponse loginResponse = await apiContext.PostAsync("https://rahulshettyacademy.com/api/ecom/auth/login", new() { DataObject = loginPayload });

            Console.WriteLine($"Status Code:{loginResponse.Status}");
            string responseBody = await loginResponse.TextAsync();
            Console.WriteLine(responseBody); //It won't work because [OneTimeSetup] does not prints any thing in console.

            // Assertion to verify whether response status is successful.
            await Expect(loginResponse).ToBeOKAsync();

            /*
                - Store complete JSON response
                - JsonAsync() reads response body as JSON.
                - JsonElement? because Playwright returns nullable JSON means respose may contain null.
            */
            JsonElement? loginResponseJson = await loginResponse.JsonAsync();

            // Fetch token later
            // Here, we are using variable which has been declared earlier so no need to declare data type.
            token = loginResponseJson
                                .Value
                                .GetProperty("token") // Fetches property from JSON. Here, token is being fetched.
                                .GetString()!;        // Converts JSON value into C# string.

        }

        [Test]
        public async Task E2E()
        {
            var Browser = await Playwright.Chromium.LaunchAsync(new()
            {
                Headless = true
            });
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();//Test


            Console.WriteLine("Token value: " + token);

            await Page.AddInitScriptAsync($@"window.localStorage.setItem('token', '{token}');");
            /*  
                Here, we want to store token value directly in local storage of browser page. And browser local storage is empty initially.            
                * AddInitScriptAsync() - Runs JavaScript before page loads and token gets inseryed before application starts.
                                       - Once application starts, it thinks user is already logged in.
             
                * AddInitScriptAsync() and EvaluateAsync() are browser JavaScript execution methods.
                * They are used to run JavaScript inside browser page/context.
                 
                * AddInitScriptAsync() injects JS before page loads.
                * EvaluateAsync() executes JS after page loads.
            */


            await Page.GotoAsync("https://rahulshettyacademy.com/client");
            /*  Now, login detail are not required as we are bypassing login scenario.
             
            await Page.Locator("#userEmail").FillAsync("prateik.mallik@outlook.com");
            await Page.Locator("#userPassword").FillAsync("Dukekumar@96");
            await Page.Locator("#login").ClickAsync();
            await products.Last.WaitForAsync();*/


            ILocator products = Page.Locator("div.card-body");
            string productName = "ZARA COAT 3";
            ILocator cartBtn = Page.Locator("[routerlink=\"/dashboard/cart\"]");

            ILocator myCart = Page.Locator(".cartSection h3");

            int productCount = await products.CountAsync(); //- returns the count of elements matching the locator

            for (int i = 0; i < productCount; i++)
            {

                if (await products.Nth(i).Locator("b").TextContentAsync() == productName)
                {
                    await products.Nth(i).Locator("i.fa.fa-shopping-cart").ClickAsync();
                    break;

                    //This same can be achived via:
                    //await products.Nth(i).Locator("text = Add To Cart").ClickAsync();
                    //break;
                }
            }

            await Expect(Page.Locator("text=Product Added To Cart")).ToBeVisibleAsync();
            Console.WriteLine(await Page.Locator("text=Product Added To Cart").TextContentAsync());

            await cartBtn.ClickAsync();

            await Page.Locator("div li").Last.WaitForAsync();

            int myCartCount = await myCart.CountAsync();
            Console.WriteLine(myCartCount);
            for (int i = 0; i < myCartCount; i++)
            {
                if (await myCart.Nth(i).TextContentAsync() == productName)
                {
                    Console.WriteLine(await myCart.Nth(i).TextContentAsync());
                }
            }

            //Below is the another way of verifying if product is present or not inside the cart section:
            bool Bool = await Page.Locator("h3:has-text('ZARA COAT 3')").IsVisibleAsync();
            Assert.That(Bool, Is.True);

            /*
                - Assert.That() is a part of NUnit.
                - expect().toBeTruthy() is a part of Playwright Node.js
                - Expect(locator).ToBeVisibleAsync() is a part of Playwright .NET. It is recommended to use this if using Playwright.
            */


            await Page.Locator("text=Checkout").ClickAsync();

            await Page.Locator("div.field [class*='validated']").PressSequentiallyAsync("1234123412341234");
            await Page.Locator("div.field.small [class='input txt']").FillAsync("123");
            await Page.Locator("div.field [class='input txt']").Last.FillAsync("Prateik");

            ILocator selectMonth = Page.Locator(".input.ddl").First;
            await selectMonth.SelectOptionAsync("06");

            ILocator selectDate = Page.Locator(".input.ddl").Last;
            await selectDate.SelectOptionAsync("25");


            await Page.Locator("[name = 'coupon']").FillAsync("rahulshettyacademy");
            await Page.Locator("button[type='submit']").ClickAsync();
            //await Page.Locator("text=Apply Coupon").ClickAsync();

            await Expect(Page.Locator("p.mt-1.ng-star-inserted")).ToBeVisibleAsync();
            await Expect(Page.Locator("p.mt-1.ng-star-inserted")).ToContainTextAsync("* Coupon Applied");
            Console.WriteLine(await Page.Locator("p.mt-1.ng-star-inserted").TextContentAsync());


            await Expect(Page.Locator("div.user__name label")).ToContainTextAsync("prateik.mallik@outlook.com");
            Console.WriteLine(await Page.Locator("div.user__name label").TextContentAsync());

            await Page.Locator("[placeholder*='Country']").PressSequentiallyAsync("ind");
            ILocator dropdown = Page.Locator(".ta-results");
            await dropdown.Locator("button").Last.WaitForAsync();

            string countryName = " India";

            int dpCounts = await dropdown.Locator("button").CountAsync();

            for (int i = 0; i < dpCounts; i++)
            {
                if (await dropdown.Locator("button").Nth(i).TextContentAsync() == countryName)
                {
                    await dropdown.Locator("button").Nth(i).ClickAsync();
                    break;
                }
            }

            await Page.Locator("a.btnn.action__submit.ng-star-inserted").ClickAsync();

            await Page.Locator("h1.hero-primary").WaitForAsync();

            string successfulText = " Thankyou for the order. ";
            await Expect(Page.Locator("h1.hero-primary")).ToHaveTextAsync(successfulText);

            ILocator orderId_ = Page.Locator("label.ng-star-inserted");

            string? orderId = await orderId_.TextContentAsync();

            orderId = orderId?.Trim('|', ' ');

            Console.WriteLine(orderId);

            await Page.Locator("label[routerlink='/dashboard/myorders']").ClickAsync();



            ILocator orderRow = Page.Locator("tr.ng-star-inserted");
            //ILocator viewBtn = Page.Locator("tr.ng-star-inserted button.btn.btn-primary");

            await orderRow.Last.WaitForAsync();
            int orderRowCount = await orderRow.CountAsync();

            for (int i = 0; i < orderRowCount; i++)
            {
                if (await orderRow.Nth(i).Locator("th").TextContentAsync() == orderId)
                {
                    await orderRow.Nth(i).Locator("td button.btn.btn-primary").ClickAsync();
                    break;
                }

                await Expect(Page.Locator("div div.col-text.-main")).ToHaveTextAsync(orderId);
            }

        }
    }
}
