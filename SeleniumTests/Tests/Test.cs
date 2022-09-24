using OpenQA.Selenium.Remote;
using SeleniumTests.Fixtures;
using SeleniumTests.Extensions;

namespace SeleniumTests.Tests
{
    [Collection(BaseRemoteFixture.CollectionName)]
    public class Test : IClassFixture<BaseRemoteFixture>
    {
        private readonly RemoteWebDriver driver;

        public Test(BaseRemoteFixture fixture)
        {
            driver = fixture.WebDriver;
        }

        [Fact]
        public void UserIsAbleToOpenWelcomePage()
        {
            driver.GoToWelcomePage();
            var t = driver.FindElementByClassName("page");
            driver.Quit();
        }
    }
}