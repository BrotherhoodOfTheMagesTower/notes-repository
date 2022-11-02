using OpenQA.Selenium;
using SeleniumTests.Infrastructure;
using SeleniumTests.Pages;
using SeleniumTests.Pages.Note;

namespace SeleniumTests.PageObjects.Modals;

public class SaveNoteModal
{
	private readonly IWebElement modal;
	private readonly IWebDriver driver;
	private readonly By modalSelector = By.XPath("//div[@class='blazored-modal']");
	private readonly By noteTitle = By.Id("noteTitle");

	public SaveNoteModal(IWebDriver driver)
	{
        this.driver = driver;
        modal = driver.WaitUntilElementExists(modalSelector);
    }

	public SaveNoteModal InsertTitle(string title)
	{
		var input = modal.FindElement(noteTitle);
		input.Clear();
		input.SendKeys(title);

		return this;
    }
	
	public HomePage ClickSave()
	{
		modal.FindElement(By.XPath("//button[text()='Save']")).Click();

		return new HomePage(driver);
    }
}
