using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playwright_learnings
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]

    /*
            - It is adviced to use single type of locator across framework to maintain consistency.
            - Mostly, css or xpath is used.
    */
    public class Test4_GetBy : CloudBrowserPageTest
    {
        [Test]
        public async Task GetByLabel()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            await Page.GotoAsync("https://rahulshettyacademy.com/angularpractice/");

            await Page.GetByLabel("Check me out if you Love IceCreams!").ClickAsync();
            await Page.GetByLabel("Employed").CheckAsync();
            await Page.GetByLabel("Gender").SelectOptionAsync("Male");

            /*
                - GetByLabel is mostly used for clicking/checking operations.
                - It is recommended not to use GetByLabel for filling operations because:
                        - Used for label
                        - For clicking/checking operations, if user clicks on text and click/check box gets highlighted then GetByLabel can be used.
                        - For filling operations, it has been seen that Playwright was not able to search for text box nearby.

            */
        }
        [Test]
        public async Task GetByPlaceholder()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();
            await Page.GotoAsync("https://rahulshettyacademy.com/angularpractice/");

            await Page.GetByPlaceholder("Password").FillAsync("abc123");

            //Placeholder should be present inside HTML.
        }

        [Test]
        public async Task GetByRole()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();
            await Page.GotoAsync("https://rahulshettyacademy.com/angularpractice/");

            await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
            //Playwright will filter all the button it sees from the webpage then search for Button havin "Submit" as name.
            //AriaRole.Button can be used if button tag is present or btn in class name.

            await Page.GetByRole(AriaRole.Link, new() { Name = "Shop" }).ClickAsync();
            Console.WriteLine(Page.Url);
        }

        [Test]
        public async Task GetByText()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();
            await Page.GotoAsync("https://rahulshettyacademy.com/angularpractice/");

            await Page.GetByText("Shop").ClickAsync(); //It will search for the element by text and perform required operation.
            Console.WriteLine(Page.Url);
        }

        [Test]
        public async Task Filter()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            await Page.GotoAsync("https://rahulshettyacademy.com/angularpractice/shop");
            await Page.Locator("app-card").Filter(new() { HasText = "Nokia Edge" }).GetByRole(AriaRole.Button).ClickAsync();

            await Page.PauseAsync();

            /*
             - Here, instead of storing all options in a list and then iterating through it, Filter was used to get the desired option.
             - Once desired option was fetched, GetByRole was used to find the button.
             - Since there is only single button for selected option so no need to mention name of the button explicitly.
            */

        }
        
    }
}
