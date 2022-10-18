using OpenQA.Selenium;

namespace SeleniumTests.Pages;

public class HomePage
{
    private IWebDriver driver;

	public HomePage(IWebDriver driver)
	{
		this.driver = driver;
	}
}
