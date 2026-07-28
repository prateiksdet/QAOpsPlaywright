using Azure.Developer.Playwright.NUnit;
using Azure.Identity;
using NUnit.Framework;

namespace Playwright_learnings;

[SetUpFixture]
public class PlaywrightServiceNUnitSetup : PlaywrightServiceBrowserNUnit
{
    public PlaywrightServiceNUnitSetup()
        : base(new DefaultAzureCredential())
    {
    }
}
