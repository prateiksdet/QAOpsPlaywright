using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playwright_learnings
{
    internal class Test11_Browsers : CloudBrowserPageTest
    {

        /*
            * Do I need only one page? 
                    - Use PageTest
            * Do I need multiple tabs/pages in the same session?
                    - Use ContextTest
            * Do I need multiple isolated users/sessions?
                    - Use BrowserTest
            * Do I need to launch browsers myself (Chrome + Firefox, custom setup)?
                    - Use PlaywrightTest
        */

        string username = "#userEmail";
        string password = "#userPassword";
        string login = "#login";

        [Test]
        public async Task Context_Test()
        {

            /*var Page1 = await Context.NewPageAsync();
            var Page2 = await Context.NewPageAsync();

            await Page1.GotoAsync("https://rahulshettyacademy.com/client/#/auth/login");
            await Page1.Locator(username).FillAsync("prateik.mallik@outlook.com");
            await Page1.Locator(password).FillAsync("Dukekumar@96");
            await Page1.Locator(login).ClickAsync();

            await Page2.GotoAsync("https://rahulshettyacademy.com/client/#/auth/login");
            await Page2.PauseAsync();
*/
            /*
               - When using ContextTest
                    * All the pages use same Cookies, Session and Local Storage because they belong to same context.
                    * Here, if Page1 is logged in and user opens new page then that will also be logged in already.
             */
        }


        [Test]
        public async Task Browser_Test()
        {
            /*var context1 = await Browser.NewContextAsync();
            var context2 = await Browser.NewContextAsync();

            var Page1 = await context1.NewPageAsync();
            var Page2 = await context2.NewPageAsync();

            await Page1.GotoAsync("https://rahulshettyacademy.com/client/#/auth/login");
            await Page1.Locator(username).FillAsync("prateik.mallik@outlook.com");
            await Page1.Locator(password).FillAsync("Dukekumar@96");
            await Page1.Locator(login).ClickAsync();

            await Page2.GotoAsync("https://rahulshettyacademy.com/client/#/auth/login");
            await Page2.PauseAsync();*/

            /*
               - When using BrowserTest
                    * No pages use same Cookies, Session and Local Storage because they belong to same context.
                    * Here, if Page1 is logged in and user opens new page then that will not be logged in.
             */
        }


        [Test]
        public async Task Playwright_Test()
        {
            var chrome_browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            var firefox_browser = await Playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            var context1 = await chrome_browser.NewContextAsync();
            var context2 = await firefox_browser.NewContextAsync();
            var Page1 = await context1.NewPageAsync();
            var Page2 = await context2.NewPageAsync();


            await Page1.GotoAsync("https://rahulshettyacademy.com/client/#/auth/login");

            await Page2.GotoAsync("https://rahulshettyacademy.com/client/#/auth/login");
            //await Page2.PauseAsync();


            /*
               - When using PlaywrightTest
                    * Multiple number of browsers needs to be used.
             */
        }
    }
}
