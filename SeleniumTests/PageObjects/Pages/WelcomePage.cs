using OpenQA.Selenium;

namespace SeleniumTests.Pages;

public class WelcomePage
{
	private IWebDriver driver;

	public WelcomePage(IWebDriver driver)
	{
		this.driver = driver;
	}
}
