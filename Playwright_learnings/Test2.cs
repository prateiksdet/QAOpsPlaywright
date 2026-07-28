using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace Playwright_learnings
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    /* 
       - [TestFixture] represents a test class which contains test methods. 
                - It is used to group related tests together and provide a common setup and teardown for those tests.
       
       - [Test] represents an individual test method that will be executed as part of the test suite. 
     
     
     
       1. [Parallelizable(ParallelScope.Self)]:
                - Test class can run in parallel with other test classes.
                - [Test] methods inside this class run sequentially.
                - It can be applied to both classes and methods, but it is intended for classes.
        
       2. [Parallelizable(ParallelScope.Children)]:
                - [Test] methods inside this class can run in parallel with each other.
                - The class itself is NOT marked for fixture-level parallelism.
         
       3. [Parallelizable(ParallelScope.Fixtures)]:
                - Test classes can run in parallel with other test classes.
                - [Test] methods inside each class run sequentially.
                - It is intended for classes.

       4. [Parallelizable(ParallelScope.All)]:
                - Test classes can run in parallel with other test classes.
                - [Test] methods inside class can also run in parallel with each other.

    */
    public class Test2: CloudBrowserPageTest
    {

        [Test]
        [Explicit]
        public async Task ChildWindows()
        {
            var context = await Browser.NewContextAsync();
            var Page = await context.NewPageAsync();
            ILocator link1 = Page.Locator("[target='_blank']").First;
            await Page.GotoAsync("https://rahulshettyacademy.com/loginpagePractise/");

            //Need to give knowledge to playwright that we are going to click on a link which will open a new tab, so that it can handle the new tab and switch to it.

            /*await link1.ClickAsync();
            var newPage = await context.WaitForPageAsync(); -- Unsafe way to handle new tab, because Playwright is asynchronous:
                                                                    - Click may instantly open new tab
                                                                    - Playwright starts waiting too late
                                                                    - If there are multiple links which open new tabs, then it will not know which one to switch to.*/

            IPage Page2 = await context.RunAndWaitForPageAsync(async () =>
            {
                await link1.ClickAsync();
            });
            /*- starts waiting internally
              - executes action
              - captures new page automatically*/

            Console.WriteLine(await Page2.TitleAsync());

        }
    }
}
