using OpenQA.Selenium.Remote;
using SeleniumTests.Constants;
using SeleniumTests.Pages;

namespace SeleniumTests.Extensions;

public static class Navigator
{
    public static WelcomePage GoToWelcomePage(this RemoteWebDriver driver)
    {
        driver.Navigate().GoToUrl(Urls.baseUrl);

        return new WelcomePage(driver);
    }
    
    public static WelcomePage GoToLoginPage(this RemoteWebDriver driver)
    {
        driver.Navigate().GoToUrl($"{Urls.baseUrl}/Identity/Account/Login");

        return new WelcomePage(driver);
    }
}
