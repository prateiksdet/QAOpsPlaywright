using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Playwright_learnings
{
    [TestFixture]
    public class Test3_E2E: CloudBrowserPageTest
    {
        [Test]
        public async Task E2E()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            ILocator products = Page.Locator("div.card-body");
            string productName = "ZARA COAT 3";
            ILocator cartBtn = Page.Locator("[routerlink=\"/dashboard/cart\"]");

            ILocator myCart = Page.Locator(".cartSection h3");

            await Page.GotoAsync("https://rahulshettyacademy.com/client/#/auth/login");
            await Page.Locator("#userEmail").FillAsync("prateik.mallik@outlook.com");
            await Page.Locator("#userPassword").FillAsync("Dukekumar@96");
            await Page.Locator("#login").ClickAsync();

            await products.Last.WaitForAsync();

            int productCount = await products.CountAsync(); //- returns the count of elements matching the locator

            for(int i =0; i<productCount; i++)
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
            for (int i = 0; i<myCartCount; i++)
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

            for (int i=0; i<dpCounts; i++)
            {
                if (await dropdown.Locator("button").Nth(i).TextContentAsync() == countryName )
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
