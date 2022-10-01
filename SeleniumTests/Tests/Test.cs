using OpenQA.Selenium.Remote;
using SeleniumTests.Fixtures;
using SeleniumTests.Extensions;
using SeleniumTests.Infrastructure.Seeders;

namespace SeleniumTests.Tests
{
    public class Test : IClassFixture<BaseRemoteFixture>
    {
        private readonly RemoteWebDriver driver;
        private BasicSeedingReport report;

        public Test(BaseRemoteFixture fixture)
        {
            driver = fixture.WebDriver;
            report = fixture.BasicSeedingReport;
        }

        [Fact]
        public void UserIsAbleToOpenWelcomePage()
        {
            var accounts = report.Users.Select(x => x.Name);
            driver.GoToWelcomePage();
            var t = driver.FindElementByClassName("page");
            driver.Quit();
        }
    }
}