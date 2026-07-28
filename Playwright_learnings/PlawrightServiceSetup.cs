using Azure.Developer.Playwright.NUnit;
using NUnit.Framework;
using Azure.Identity;

namespace Playwright_learnings
{
    internal class PlawrightServiceSetup
    {
        [SetUpFixture]
        public class PlaywrightServiceNUnitSetup : PlaywrightServiceBrowserNUnit
        {
            public PlaywrightServiceNUnitSetup() : base(
                credential: new DefaultAzureCredential()
            )
            { }
        }
    }
}
