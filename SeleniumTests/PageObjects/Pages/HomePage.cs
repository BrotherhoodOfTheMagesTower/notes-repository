using NotesRepository.Pages.LoggedUser;
using OpenQA.Selenium;
using SeleniumTests.Infrastructure;
using SeleniumTests.Pages.Note;

namespace SeleniumTests.Pages;

public class HomePage
{
    private IWebDriver driver;
	private readonly By newNoteButton = By.XPath("//a[@data-ref='new-note-btn']");
	private readonly By foldersButton = By.XPath("//a[@data-ref='main-dir-btn']");
	private readonly By sharedButton = By.XPath("//a[@data-ref='shared-btn']");
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

	public PageObjects.Pages.Note.SharedNotesPage ClickSharedFromNavMenu()
	{
		driver.WaitUntilElementExists(sharedButton);
        driver.FindElement(sharedButton).Click();

		return new PageObjects.Pages.Note.SharedNotesPage(driver);
	}

	public EditNotePage InsertIntoSearchBarAndClickResult(string attribute)
	{
		driver.WaitUntilElementExists(searchBar).SendKeys(attribute);
		driver.WaitUntilElementExists(ResultFromSearchBar(attribute));
		driver.FindElement(ResultFromSearchBar(attribute)).Click();

		return new EditNotePage(driver);
	}
}
