using OpenQA.Selenium;

namespace SeleniumTests.Pages.Account;

public class LoginPage
{
    private IWebDriver driver;

    private By InputSelector(string dataRef) => By.XPath($"//input[@data-ref='{dataRef}']");
    private By summaryValidation => By.XPath("//div[@data-ref='summary-validation']");
    private By loginButton => By.XPath("//button[@data-ref='login']");
    private By usernameInput => InputSelector("username");
    private By passwordInput => InputSelector("password");

    public LoginPage(IWebDriver driver)
    {
        this.driver = driver;
    }

    public LoginPage InsertEmail(string email)
    {
        driver.FindElement(usernameInput).SendKeys(email);

        return this;
    }

    public LoginPage InsertPassword(string password)
    {
        driver.FindElement(passwordInput).SendKeys(password);

        return this;
    }

    public LoginPage LoginAndStay()
    {
        driver.FindElement(loginButton).Click();

        return this;
    }
    
    public HomePage Login()
    {
        driver.FindElement(loginButton).Click();

        return new HomePage(driver);
    }

    public string GetSummaryErrors() => driver.FindElement(summaryValidation).Text;
}
