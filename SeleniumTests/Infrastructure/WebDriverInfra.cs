using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using SeleniumTests.Constants;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests.Infrastructure;

public static class WebDriverInfra
{
    public static IWebDriver CreateInstance(BrowserType browserType)
    {
        switch (browserType)
        {
            case BrowserType.Chrome:
                return new ChromeDriver();
            case BrowserType.Edge:
                return new EdgeDriver();
            default:
                throw new ArgumentOutOfRangeException(nameof(browserType), browserType, null);
        }
    }

    public static IWebDriver CreateInstance(BrowserType browserType, string hubUrl)
    {
        switch (browserType)
        {
            case BrowserType.Chrome:
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("no-sandbox");
                return GetWebDriver(hubUrl, chromeOptions.ToCapabilities());
            case BrowserType.Edge:
                EdgeOptions options = new EdgeOptions();
                return GetWebDriver(hubUrl, options.ToCapabilities());
            default: 
                throw new ArgumentOutOfRangeException(nameof(browserType), browserType, null);
        }
    }

    public static IWebElement WaitUntilElementExists(this IWebDriver driver, By elementLocator, int timeout = 10)
    {
        try
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            return wait.Until(x => x.FindElement(elementLocator));
        }
        catch (NoSuchElementException)
        {
            Console.WriteLine("Element with locator: '" + elementLocator + "' was not found in current context page.");
            throw;
        }
    }

    private static IWebDriver GetWebDriver(string hubUrl, ICapabilities capabilities)
    {
        TimeSpan timeSpan = new TimeSpan(0, 0, 3);
        var urio = new Uri(hubUrl);
        var rmWeb = new RemoteWebDriver(
                    urio,
                    capabilities
                );
        return rmWeb;
    }
}
