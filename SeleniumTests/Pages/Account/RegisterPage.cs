using OpenQA.Selenium;

namespace SeleniumTests.Pages.Account;

public class RegisterPage
{
    private IWebDriver driver;

    private By InputSelector(string dataRef) => By.XPath($"//input[@data-ref='{dataRef}']");
    private By registerButton => By.XPath($"//button[@data-ref='register']");
    private By firstNameInput => InputSelector("first-name");
    private By lastNameInput => InputSelector("last-name");
    private By emailInput => InputSelector("e-mail");
    private By passwordInput => InputSelector("password");
    private By confirmPasswordInput => InputSelector("confirm-password");

    public RegisterPage(IWebDriver driver)
    {
        this.driver = driver;
    }

    public RegisterPage InsertFirstName(string firstName)
    {
        driver.FindElement(firstNameInput).SendKeys(firstName);

        return this;
    }
    
    public RegisterPage InsertLastName(string lastName)
    {
        driver.FindElement(lastNameInput).SendKeys(lastName);

        return this;
    }
    
    public RegisterPage InsertEmail(string email)
    {
        driver.FindElement(emailInput).SendKeys(email);

        return this;
    }
    
    public RegisterPage InsertPassword(string password)
    {
        driver.FindElement(passwordInput).SendKeys(password);

        return this;
    }
    
    public RegisterPage InsertConfirmedPassword(string confirmedPassword)
    {
        driver.FindElement(confirmPasswordInput).SendKeys(confirmedPassword);

        return this;
    }
    
    public HomePage Register()
    {
        driver.FindElement(registerButton).Click();

        return new HomePage(driver);
    }
}
