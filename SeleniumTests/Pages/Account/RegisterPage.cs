using OpenQA.Selenium;

namespace SeleniumTests.Pages.Account;

public class RegisterPage
{
    private IWebDriver driver;

    private By InputSelector(string dataRef) => By.XPath($"//input[@data-ref='{dataRef}']");
    private By summaryValidation => By.XPath("//div[@data-ref='summary-validation']");
    private By confirmationPasswordValidation => By.XPath("//span[@data-ref='confirm-password-validation']");
    private By passwordValidation => By.XPath("//span[@data-ref='password-validation']");
    private By emailValidation => By.XPath("//span[@data-ref='email-validation']");
    private By registerButton => By.XPath("//button[@data-ref='register']");
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
    
    public RegisterPage RegisterAndStay()
    {
        driver.FindElement(registerButton).Click();

        return this;
    }

    public string GetSummaryErrors() => driver.FindElement(summaryValidation).Text;

    public string GetConfirmationPasswordErrors() => driver.FindElement(confirmationPasswordValidation).Text;

    public string GetPasswordErrors() => driver.FindElement(passwordValidation).Text;

    public string GetEmailErrors() => driver.FindElement(emailValidation).Text;
}
