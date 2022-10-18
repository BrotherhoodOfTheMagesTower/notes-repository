using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SeleniumTests.Constants;
using SeleniumTests.Pages;
using SeleniumTests.Pages.Account;

namespace SeleniumTests.Extensions;

public static class Navigator
{
    public static WelcomePage GoToWelcomePage(this IWebDriver driver)
    {
        driver.Navigate().GoToUrl(Urls.baseUrl);

        return new WelcomePage(driver);
    }

    public static LoginPage GoToLoginPage(this IWebDriver driver)
    {
        driver.Navigate().GoToUrl($"{Urls.baseUrl}/Identity/Account/Login");

        return new LoginPage(driver);
    }
    
    public static RegisterPage GoToRegisterPage(this IWebDriver driver)
    {
        driver.Navigate().GoToUrl($"{Urls.baseUrl}/Identity/Account/Register");

        return new RegisterPage(driver);
    }
    
    public static AccountManagementPage GoToAccountManagement(this IWebDriver driver)
    {
        driver.Navigate().GoToUrl($"{Urls.baseUrl}/Identity/Account/Manage");

        return new AccountManagementPage(driver);
    }
}
