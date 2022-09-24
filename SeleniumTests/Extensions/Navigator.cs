using OpenQA.Selenium.Remote;
using SeleniumTests.Pages;

namespace SeleniumTests.Extensions;

public static class Navigator
{
    private const string baseUrl = "http://host.docker.internal:8000";

    public static WelcomePage GoToWelcomePage(this RemoteWebDriver driver)
    {
        driver.Navigate().GoToUrl(baseUrl);

        return new WelcomePage(driver);
    }
    
    public static WelcomePage GoToLoginPage(this RemoteWebDriver driver)
    {
        driver.Navigate().GoToUrl($"{baseUrl}/Identity/Account/Login");

        return new WelcomePage(driver);
    }
}
