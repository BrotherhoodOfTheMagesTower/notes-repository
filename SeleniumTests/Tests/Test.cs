using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using SeleniumTests.Constants;
using SeleniumTests.Infrastructure;
using System;

namespace SeleniumTests.Tests
{
    public class Test
    {
        private IWebDriver driver;
        private string hubUrl;

        [Theory]
        [InlineData(BrowserType.Chrome)]
        public void Test1(BrowserType browser)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            var driver = new RemoteWebDriver(new Uri("http://localhost:4445"), chromeOptions);
            driver.Navigate().GoToUrl("http://localhost:8000");
            var t = driver.FindElementByClassName("page");
            var halo =  t.Text;
            driver.Quit();
        }
    }
}