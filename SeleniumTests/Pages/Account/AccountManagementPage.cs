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
	private By updateButton => By.XPath("//button[@data-ref='update']");

    public AccountManagementPage(IWebDriver driver)
	{
		this.driver = driver;
	}

	public string GetUsername() => driver.FindElement(username).GetAttribute("value");

    public string GetFirstName() => driver.FindElement(firstName).GetAttribute("value");

	public string GetLastName() => driver.FindElement(lastName).GetAttribute("value");

    public string GetPhoneNumber() => driver.FindElement(phoneNumber).GetAttribute("value");
}
