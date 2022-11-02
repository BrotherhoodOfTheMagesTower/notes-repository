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
	private readonly By calendarButton = By.XPath("//a[@data-ref='calendar-btn']");
	private readonly By searchBar = By.XPath("//input[@data-ref='search-bar']");
	private By ResultFromSearchBar(string attribute) => By.XPath($"//div[text()='{attribute}']");

	public HomePage(IWebDriver driver)
	{
		this.driver = driver;
	}

	public EditNotePage ClickNewNoteFromNavMenu()
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
	
	public PageObjects.Pages.CalendarPage ClickCalendarFromNavMenu()
	{
		driver.WaitUntilElementExists(calendarButton);
        driver.FindElement(calendarButton).Click();

		return new PageObjects.Pages.CalendarPage(driver);
	}

	public EditNotePage InsertIntoSearchBarAndClickResult(string attribute)
	{
		var search = driver.WaitUntilElementExists(searchBar);
		foreach(var letter in attribute)
            search.SendKeys(letter.ToString());
		driver.WaitUntilElementExists(ResultFromSearchBar(attribute));
		driver.FindElement(ResultFromSearchBar(attribute)).Click();

		return new EditNotePage(driver);
	}
}
