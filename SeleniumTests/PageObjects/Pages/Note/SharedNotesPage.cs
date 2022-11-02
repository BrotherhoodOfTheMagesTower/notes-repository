using OpenQA.Selenium;
using SeleniumTests.Infrastructure;
using SeleniumTests.Pages.Note;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTests.PageObjects.Pages.Note;

public class SharedNotesPage
{
	private IWebDriver driver;
	private readonly By sharedNotesTitle = By.XPath("//div[@data-ref='shared-note-title']");
	private readonly By sharedNotesContent = By.XPath("//div[@data-ref='shared-note-content']");
	private readonly By sharedNotesHeader = By.XPath("//h2[text()='Shared notes']");

	public SharedNotesPage(IWebDriver driver)
	{
		this.driver = driver;
		driver.WaitUntilElementExists(sharedNotesHeader);
	}

	public string[] GetAllVisibleSharedNoteTitles()
		=> driver.FindElements(sharedNotesTitle).Select(x => x.Text).ToArray();
	
	public string[] GetAllVisibleSharedNoteContents()
		=> driver.FindElements(sharedNotesContent).Select(x => x.Text).ToArray();

	public EditNotePage ClickFirstSharedNote(bool waitForEditNotePage = true)
	{
		driver.FindElement(sharedNotesContent).Click();

		return new EditNotePage(driver, waitForEditNotePage);

    }
}
