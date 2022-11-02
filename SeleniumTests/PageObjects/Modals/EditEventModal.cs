using OpenQA.Selenium;
using Polly;
using Quartz.Util;
using SeleniumTests.Infrastructure;
using SeleniumTests.PageObjects.Pages;
using SeleniumTests.Pages.Note;

namespace SeleniumTests.PageObjects.Modals;

public class EditEventModal
{
    private IWebDriver driver;
    private IWebElement modal;

    private readonly By editEventModal = By.XPath("//div[@class='rz-dialog']");
    private readonly By noteSelect = By.TagName("select");
    private readonly By selectedNoteOption = By.XPath("//option[@selected]");
    private readonly By titleInput = By.XPath("//input[@name='Content']");
    private readonly By startAtInput = By.XPath("//input[@name='StartAt']");
    private readonly By endAtInput = By.XPath("//input[@name='EndAt']");
    private readonly By saveButton = By.XPath("//button[@type='submit']");
    private readonly By toastMessage = By.ClassName("blazored-toast-message");
    private readonly By goToNoteButton = By.XPath("//button[text()='Go to this note']");
    private readonly By deleteEventButton = By.XPath("//button[@data-ref='delete-button']");
    private By NoteSelectOption(string title) => By.XPath($"//option[text()='{title}']");

    public EditEventModal(IWebDriver driver)
	{
		this.driver = driver;
        modal = driver.WaitUntilElementExists(editEventModal);
    }
    public EditEventModal WithTitle(string title)
    {
        var input = driver.WaitUntilElementExists(titleInput);
        input.SendKeys(Keys.Control + "a" + Keys.Delete);
        foreach (var letter in title)
            input.SendKeys($"{letter}");

        return this;
    }

    public EditEventModal WithStartAt(string startAt)
    {
        var input = driver.WaitUntilElementExists(startAtInput);
        input.SendKeys(Keys.Control + "a" + Keys.Delete);
        foreach (var letter in startAt)
            modal.FindElement(startAtInput).SendKeys($"{letter}");

        return this;
    }

    public EditEventModal WithEndAt(string endAt)
    {
        var input = driver.WaitUntilElementExists(endAtInput);
        input.SendKeys(Keys.Control + "a" + Keys.Delete);
        foreach (var letter in endAt)
            modal.FindElement(endAtInput).SendKeys($"{letter}");

        return this;
    }

    public EditEventModal WithNote(string noteTitle)
    {
        modal.FindElement(noteSelect).Click();
        modal.FindElement(NoteSelectOption(noteTitle)).Click();

        return this;
    }
    
    public EditNotePage GoToAttachedNote()
    {
        driver.WaitUntilElementExists(goToNoteButton).Click();

        return new EditNotePage(driver);
    }

    public async Task<string> SaveAndGetNotification()
    {
        await Task.Delay(1000); //wait for clickable Save Button
        modal.FindElement(saveButton).Click();
        driver.WaitUntilElementExists(toastMessage);
        var notificationContent = Policy
            .HandleResult<string>(x => x.IsNullOrWhiteSpace())
            .WaitAndRetry(retryCount: 10, sleepDurationProvider: _ => TimeSpan.FromSeconds(1))
            .Execute(() => driver.FindElement(toastMessage).Text);

        return notificationContent;
    }
    
    public async Task<CalendarPage> Save()
    {
        await Task.Delay(1000); //wait for clickable Save Button
        modal.FindElement(saveButton).Click();

        return new CalendarPage(driver);
    }
    
    public string DeleteEventAndGetNotification()
    {
        modal.FindElement(deleteEventButton).Click();
        driver.WaitUntilElementExists(toastMessage);
        var notificationContent = Policy
            .HandleResult<string>(x => x.IsNullOrWhiteSpace())
            .WaitAndRetry(retryCount: 10, sleepDurationProvider: _ => TimeSpan.FromSeconds(1))
            .Execute(() => driver.FindElement(toastMessage).Text);

        return notificationContent;
    }

    public string GetSelectedNoteTitle() => modal.FindElement(selectedNoteOption).GetAttribute("innerHTML");
}
