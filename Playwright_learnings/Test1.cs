using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playwright_learnings
{
    internal class Test1 : CloudBrowserPageTest
    {
        [Test]
        [Explicit]

        /*
            Why async and Task?
                - In Playwright, most methods are asynchronous(async) in nature and that's why async and Task are used.
                - async is used so that 'await' can be used.
                - Task allows non-blocking execution while waiting. 
        */

        public async Task Method1()
        {
            await Page.GotoAsync("https://www.google.com");
            Console.WriteLine(await Page.TitleAsync());

            await Expect(Page).ToHaveTitleAsync("Google");
        }

        [Test]
        [Explicit]
        public async Task AllCardsWithoutWait()
        {
            ILocator username = Page.Locator("#username");
            ILocator password = Page.Locator("input[name='password']");
            ILocator signin = Page.Locator("#signInBtn");
            ILocator cardtitles = Page.Locator(".card-body a");

            await Page.GotoAsync("https://rahulshettyacademy.com/loginpagePractise/");
            await username.FillAsync("rahulshettyacademy1");
            await password.FillAsync("learning");
            await signin.ClickAsync();
            Console.WriteLine(await Page.Locator("div[style*='block']").TextContentAsync());
            /*
            We don't write Page.Locator("div[style*='block']").TextContentAsync().ToString() because:
                - TextContentAsync() does not return text immediately.
                - It return Task<string?> which means I will give you text in future.
                - Here, ToString() is running on the Task Object, not on the actual text.
                - It will return "System.Threading.Tasks.Task`1[System.String]"
                - Correct way of using is await Page.Locator("div[style*='block']").TextContentAsync());
                     
            */
            await Expect(Page.Locator("div[style*='block']")).ToContainTextAsync("Incorrect username/password.");

            await username.FillAsync(""); // To clear input box text
            await password.FillAsync("");

            await username.FillAsync("rahulshettyacademy");
            await password.FillAsync("Learning@830$3mK2");
            await signin.ClickAsync();

            Console.WriteLine(await cardtitles.Nth(0).TextContentAsync());  // Nth(0) means selecting first element out of multiple lelements.

            IReadOnlyList<string> allTiles = await cardtitles.AllTextContentsAsync();
            string allTitlesString = string.Join(", ", allTiles);
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(allTiles));

            /*
                - IReadOnlyList<string> is a collection of string in which item can be read but not modified directly.
                - Playwright's AllTextContentsAsync() returns read-only data. That's why return type is IReadOnlyList<string>
             
                - string.Join() is used to combine multiple strings into one string.
                    - List<string> names = ["Laptop", "Mobile", "Watch"];
                      string result = string.Join(", ", names);   
                    - It returns - Laptop, Mobile, Watch
                
                - JsonSerializer.Serialize(__) converts data into JSON string.
                    - Data becomes ["Laptop","Mobile","Watch"]
            */

            Console.WriteLine(allTitlesString);
        }

        [Test]
        public async Task Login()
        {
            await Page.GotoAsync("https://rahulshettyacademy.com/client/#/auth/login");
            await Page.Locator(".login-wrapper-footer-text").ClickAsync();
            await Page.Locator("#firstName").FillAsync("Prateik");
            await Page.Locator("#lastName").FillAsync("Mallik");
            await Page.Locator("#userEmail").FillAsync("prateik.mallik@outlook.com");
            await Page.Locator("#userMobile").FillAsync("9632587410");
            await Page.Locator("#userPassword").FillAsync("Dukekumar@96");
            await Page.Locator("#confirmPassword").FillAsync("Dukekumar@96");
            await Page.Locator("input[type='checkbox']").ClickAsync();
            await Page.Locator("#userEmail").FillAsync("prateik.mallik@outlook.com");
            await Page.Locator("#userPassword").FillAsync("Dukekumar@96");
        }

        [Test]
        public async Task WaitForSelector()
        {
            ILocator cardTitles = Page.Locator("div.card-body b");

            await Page.GotoAsync("https://rahulshettyacademy.com/client/#/auth/login");
            await Page.Locator("#userEmail").FillAsync("prateik.mallik@outlook.com");
            await Page.Locator("#userPassword").FillAsync("Dukekumar@96");
            await Page.Locator("#login").ClickAsync();

            //await Page.WaitForLoadStateAsync(LoadState.NetworkIdle); //Network idle is flaky, so we can use wait for selector instead
            

            await Page.WaitForSelectorAsync("div.card-body b");
            await cardTitles.Last.WaitForAsync();
            /* In the next step, we are trying to get the text content of all the card titles, but if we do not wait for the card titles to be visible, 
               then we will get an empty list because the card titles are not yet loaded on the page. 
               So, we need to wait for the card titles to be visible before getting their text content.
            
               This can be achieved via two ways:
                1. await Page.WaitForSelectorAsync("div.card-body b")
                     - It is seletor based and waits until selector appears in DOM
                     - It returns an ElementHandle that references a specific DOM node, which may become stale after page re-rendering.
                     - Similar to Selenium's implicit wait
 
                2. await cardTitles.WaitForAsync().
                     - Re-evaluates DOM continuously
                     - Based on Playwright's auto-waiting mechanism         
             */

            IReadOnlyList<string> cards = await cardTitles.AllTextContentsAsync();
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(cards));


            //Console.WriteLine(await Page.Locator("div.card-body b").Nth(0).TextContentAsync());
        }
        [Test]
        public async Task Dropdown_Radio()
        {
            ILocator username = Page.Locator("#username");
            ILocator password = Page.Locator("input[name='password']");
            ILocator signin = Page.Locator("#signInBtn");

            await Page.GotoAsync("https://rahulshettyacademy.com/loginpagePractise/");
            await username.FillAsync("rahulshettyacademy1");
            await password.FillAsync("learning");

            ILocator dropdown = Page.Locator("select.form-control");
            await dropdown.SelectOptionAsync("consult");

            //await Page.PauseAsync();
            /* It is used to temporarily pause test execution and open Playwright Inspector for debugging, locator inspection*/

            await Page.Locator(".radiotextsty").Last.ClickAsync();
            await Page.Locator("#okayBtn").ClickAsync();

            await Expect(Page.Locator(".radiotextsty").Last).ToBeCheckedAsync();
        }
        [Test]
        public async Task LinkBlink() //blinkingText class
        {
            ILocator link1 = Page.Locator("[target='_blank']").First;
            ILocator link2 = Page.Locator("[href*='techsmarthire']");

            await Page.GotoAsync("https://rahulshettyacademy.com/loginpagePractise/");

            await Expect(link1).ToHaveClassAsync("blinkingText");
            await Expect(link2).ToHaveAttributeAsync("class", "blinkingText");

            /* class = "blinkingText" is commonly used to make text or elements blink visually using CSS animation. */
        }

        [Test]
        public async Task TextvsInput()
        {
            ILocator username = Page.Locator("#username");

            await Page.GotoAsync("https://rahulshettyacademy.com/loginpagePractise/");
            await username.FillAsync("rahulshettyacademy1");
            Console.WriteLine(await username.InputValueAsync());

            /*InputValueAsync is used to get the value of an input field, whereas TextContentAsync() is used to get the visible/internal text content of an element.
             
              TextContentAsync() usually does not work for input boxes because input text is stored in the value attribute, not as inner text.

              <input value="Hello"> retuns null for TextContentAsync() but returns "Hello" for InputValueAsync().
              <div>Hello</div> returns "Hello" for TextContentAsync() but returns null for InputValueAsync().

              TextContentAsync() reads inner text between HTML tags, whereas input fields store user-entered text inside the value attribute.

             */

        }

    }
}
