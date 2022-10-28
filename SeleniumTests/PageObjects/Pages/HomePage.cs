using OpenQA.Selenium;
using SeleniumTests.Infrastructure;
using SeleniumTests.Pages.Note;

namespace SeleniumTests.Pages;

public class HomePage
{
    private IWebDriver driver;
	private readonly By newNoteButton = By.XPath("//a[@data-ref='new-note-btn']");
	private readonly By foldersButton = By.XPath("//a[@data-ref='main-dir-btn']");
	private readonly By searchBar = By.XPath("//input[@data-ref='search-bar']");
	private By ResultFromSearchBar(string attribute) => By.XPath($"//div[text()='{attribute}']");

	public HomePage(IWebDriver driver)
	{
		this.driver = driver;
	}

	public EditNotePage ClickNewNoteButtonFromNavMenu()
	{
		driver.WaitUntilElementExists(newNoteButton);
        driver.FindElement(newNoteButton).Click();

		return new EditNotePage(driver);
	}
	
	public EditNotePage ClickFoldersFromNavMenu()
	{
		driver.WaitUntilElementExists(foldersButton);
        driver.FindElement(foldersButton).Click();

		return new EditNotePage(driver);
	}

	public EditNotePage InsertIntoSearchBarAndClickResult(string attribute)
	{
		driver.FindElement(searchBar).SendKeys(attribute);
		driver.WaitUntilElementExists(ResultFromSearchBar(attribute));
		driver.FindElement(ResultFromSearchBar(attribute)).Click();

		return new EditNotePage(driver);
	}
}
