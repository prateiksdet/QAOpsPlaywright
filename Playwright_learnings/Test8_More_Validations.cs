using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playwright_learnings
{
    [TestFixture]
    public class Test8_More_Validations : CloudBrowserPageTest
    {
        [Test]
        public async Task PageNavigations()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            await Page.GotoAsync("https://www.google.com/");
            await Page.GotoAsync("https://www.facebook.com/");
            await Page.GoBackAsync();
            await Page.GoForwardAsync();
        }

        [Test]
        public async Task Visible_Hidden()
        {

            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            ILocator inputbox = Page.Locator("#displayed-text");
            await Page.GotoAsync("https://rahulshettyacademy.com/AutomationPractice/");

            await Expect(inputbox).ToBeVisibleAsync();
            await Page.Locator("#hide-textbox").ClickAsync();
            await Expect(inputbox).ToBeHiddenAsync();         //It will confirm the current status of inputbox must hiddden.

        }
        [Test]
        public async Task JS_popup()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            await Page.GotoAsync("https://rahulshettyacademy.com/AutomationPractice/");

            Page.Dialog += async (sender, dialog) =>
            {
                Console.WriteLine(dialog.Message);

                await dialog.AcceptAsync();
            };

            await Page.ClickAsync("#confirmbtn");
        }
        [Test]
        public async Task Cursor_hover()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            await Page.GotoAsync("https://rahulshettyacademy.com/AutomationPractice/");

            await Page.Locator("#mousehover").HoverAsync(); //It is used for mouse hover

            await Page.PauseAsync();
        }

        [Test]
        public async Task Screenshot_visualcomparison()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            ILocator inputbox = Page.Locator("#displayed-text");
            await Page.GotoAsync("https://rahulshettyacademy.com/AutomationPractice/");


            await Expect(inputbox).ToBeVisibleAsync();

            //Screenshot of particular locator:
            await Page.Locator("#displayed-text").ScreenshotAsync(new LocatorScreenshotOptions
            {
                Path = "Element.png"
            });


            await Page.Locator("#hide-textbox").ClickAsync();

            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = "HomePage.png"
            });

            await Expect(inputbox).ToBeHiddenAsync();

        }

        //[Test]
        //public async Task VisualTesting()
        //{
        //    var Context = await Browser.NewContextAsync();
        //    var Page = await Context.NewPageAsync();

        //    await Page.GotoAsync("https://www.google.com/");
        //    await Page.Locator("center input.gNO89b").Last.WaitForAsync();


        //    var baselinePath = "Baseline/landing1.png";

        //    byte[] baseline = await File.ReadAllBytesAsync(baselinePath);

        //    byte[] actual = await Page.ScreenshotAsync(new()
        //    {
        //        FullPage = true
        //    });



        //    Assert.That(actual.SequenceEqual(baseline),Is.True, "Visual mismatch detected.");
            // Use ImageSharp + PixelMatch to compare pixels
        //}
    }
}
