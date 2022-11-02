using OpenQA.Selenium;
using Polly;
using Quartz.Util;
using SeleniumTests.Infrastructure;

namespace SeleniumTests.PageObjects.Modals;

public class AddEventModal
{
    private IWebDriver driver;
    private IWebElement modal;

    private readonly By addEventModal = By.XPath("//div[@class='rz-dialog']");
    private readonly By titleInput = By.XPath("//input[@name='Content']");
    private readonly By startAtInput = By.XPath("//input[@name='StartAt']");
    private readonly By endAtInput = By.XPath("//input[@name='EndAt']");
    private readonly By noteSelect = By.TagName("select");
    private readonly By saveButton = By.XPath("//button[@type='submit']");
    private readonly By toastMessage = By.ClassName("blazored-toast-message");
    private readonly By titleErrorMessage = By.XPath("//div[contains(@class, 'rz-messages-error')]");
    private By NoteSelectOption(string title) => By.XPath($"//option[text()='{title}']");

    public AddEventModal(IWebDriver driver)
    {
        this.driver = driver;
        modal = driver.WaitUntilElementExists(addEventModal);
    }

    public AddEventModal WithTitle(string title)
    {
        foreach (var letter in title)
            modal.FindElement(titleInput).SendKeys($"{letter}");

        return this;
    }

    public AddEventModal WithStartAt(string startAt)
    {
        var input = driver.WaitUntilElementExists(startAtInput);
        input.SendKeys(Keys.Control + "a" + Keys.Delete);
        foreach (var letter in startAt)
            modal.FindElement(startAtInput).SendKeys($"{letter}");

        return this;
    }

    public AddEventModal WithEndAt(string endAt)
    {
        var input = driver.WaitUntilElementExists(endAtInput);
        input.SendKeys(Keys.Control + "a" + Keys.Delete);
        foreach (var letter in endAt)
            modal.FindElement(endAtInput).SendKeys($"{letter}");

        return this;
    }

    public AddEventModal WithNote(string noteTitle)
    {
        modal.FindElement(noteSelect).Click();
        modal.FindElement(NoteSelectOption(noteTitle)).Click();

        return this;
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

    public async Task<AddEventModal> Save()
    {
        await Task.Delay(1000); //wait for clickable Save Button
        modal.FindElement(saveButton).Click();

        return this;
    }

    public string GetTitleErrorMessage() => driver.WaitUntilElementExists(titleErrorMessage).Text;
}
