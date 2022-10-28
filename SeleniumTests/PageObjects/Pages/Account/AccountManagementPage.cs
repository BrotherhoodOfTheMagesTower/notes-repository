using OpenQA.Selenium;

namespace SeleniumTests.Pages.Account;

public class AccountManagementPage
{
    private IWebDriver driver;

	private By InputSelector(string dataRef) => By.XPath($"//input[@data-ref='{dataRef}']");
	private By username => InputSelector("username");
	private By firstName => InputSelector("first-name");
	private By lastName => InputSelector("last-name");
	private By phoneNumber => InputSelector("phone-number");
	private By currentPassword => InputSelector("current-password");
	private By newPassword => InputSelector("new-password");
	private By password => InputSelector("password");
	private By confirmNewPassword => InputSelector("confirm-new-password");
	private By updateButton => By.XPath("//button[@data-ref='update']");
	private By passwordSection => By.XPath("//a[@data-ref='password']");
	private By personalDataSection => By.XPath("//a[@data-ref='personal-data']");
	private By deleteButton => By.XPath("//a[@data-ref='delete']");
	private By deleteDataAndCloseAccountButton => By.XPath("//button[@data-ref='delete-data']");
	private By updateStatusValidation => By.XPath("//div[contains(@class, 'alert') and contains(@class, 'alert-success')]");
	private By summaryValidation => By.XPath("//div[@data-ref='summary-validation']");

    public AccountManagementPage(IWebDriver driver)
	{
		this.driver = driver;
	}

	public string GetUsername() => driver.FindElement(username).GetAttribute("value");

    public string GetFirstName() => driver.FindElement(firstName).GetAttribute("value");

	public string GetLastName() => driver.FindElement(lastName).GetAttribute("value");

    public string GetPhoneNumber() => driver.FindElement(phoneNumber).GetAttribute("value");
    
	public string GetUpdateStatus() => driver.FindElement(updateStatusValidation).Text;

	public string GetSummaryErrors() => driver.FindElement(summaryValidation).Text;

	public AccountManagementPage InsertPhoneNumber(string number)
	{
		driver.FindElement(phoneNumber).SendKeys(number);

		return this;	
	}
	
	public AccountManagementPage InsertNewPassword(string currentPass, string newPass)
	{
		driver.FindElement(currentPassword).SendKeys(currentPass);
		driver.FindElement(newPassword).SendKeys(newPass);
		driver.FindElement(confirmNewPassword).SendKeys(newPass);
		driver.FindElement(updateButton).Click();

		return this;	
	}
	
	public AccountManagementPage SelectPassword()
	{
		driver.FindElement(passwordSection).Click();

		return this;	
	}
	
	public AccountManagementPage SelectPersonalData()
	{
		driver.FindElement(personalDataSection).Click();

		return this;	
	}
	
	public AccountManagementPage Update()
	{
		driver.FindElement(updateButton).Click();

		return this;	
	}
	
	public AccountManagementPage Delete()
	{
		driver.FindElement(deleteButton).Click();

		return this;	
	}
	
	public AccountManagementPage CloseAccountAndStay()
	{
		driver.FindElement(deleteDataAndCloseAccountButton).Click();

		return this;	
	}
	
	public WelcomePage CloseAccount()
	{
		driver.FindElement(deleteDataAndCloseAccountButton).Click();

		return new WelcomePage(driver);	
	}
	
	public AccountManagementPage InsertPassword(string pass)
	{
		driver.FindElement(password).SendKeys(pass);

		return this;	
	}
}
