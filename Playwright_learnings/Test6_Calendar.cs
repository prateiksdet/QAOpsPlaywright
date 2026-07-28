using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playwright_learnings
{
    [TestFixture]
    public class Test6_Calendar: CloudBrowserPageTest
    {
        [Test]
        public async Task Calendar()
        {
            int month = 06;
            int day = 25;
            int year = 2027;

            int[] expectedList = { month, day, year }; //Writing all the expected values in an array. So that it can be valudated with actual values as a part of Assertion.

            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            await Page.GotoAsync("https://rahulshettyacademy.com/seleniumPractise/#/offers");

            await Page.Locator(".react-date-picker__inputGroup").ClickAsync();
            await Page.Locator(".react-calendar__navigation__label").ClickAsync();
            await Page.Locator(".react-calendar__navigation__label").ClickAsync();

            await Page.Locator(".react-calendar__decade-view__years").GetByRole(AriaRole.Button, new() { Name = year.ToString() }).ClickAsync();
            await Page.Locator(".react-calendar__year-view__months__month").Nth(month - 1).ClickAsync();
            await Page.Locator(".react-calendar__month-view__days").GetByText(day.ToString()).ClickAsync();

            ILocator inputs = Page.Locator(".react-date-picker__inputGroup__input"); 
            // Captures date,month and year elements. It has 3 items.
            // Here, we are just grabbing the value so not using await.
            
            for(int i=0; i<expectedList.Length; i++) //It will iterate through the expectedList. Since there are 3 array items so maximum it will iterate 3 times.
            {
                string value = await inputs.Nth(i).InputValueAsync(); 
                /* We want to retrieve value present in that locator so, InputValueAsync().
                    - Here, we don't have static value, we have something added by the user. So, TextContentAsync() won't work here. 
                    - Using GetAttributeValue() also, we can fetch the value. 
                    - Here, we performing an action i.e., InputValueAsync(), so using await.
                */
                 
                //await Expect(value).t --- Here, Expect won't work as it only works for Page, Locator, API response.

                Assert.That(value, Is.EqualTo(expectedList[i].ToString()));

            }

        }
    }
}
