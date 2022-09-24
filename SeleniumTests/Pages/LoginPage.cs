using OpenQA.Selenium;

namespace SeleniumTests.Pages;

public class LoginPage
{
    private IWebDriver driver;

    public LoginPage(IWebDriver driver)
    {
        this.driver = driver;
    }
}
