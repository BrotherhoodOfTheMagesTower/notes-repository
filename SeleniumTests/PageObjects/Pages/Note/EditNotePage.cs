using OpenQA.Selenium;
using Polly;
using Quartz.Util;
using SeleniumTests.Infrastructure;
using SeleniumTests.PageObjects.Modals;

namespace SeleniumTests.Pages.Note;

public class EditNotePage
{
    private IWebDriver driver;
    private readonly By noteContentInput = By.XPath("//textarea[@data-ref='note-input']");
    private readonly By saveButton = By.XPath("//button[@data-ref='save-note']");
    private readonly By addCollaboratorButton = By.XPath("//button[@data-ref='add-collaborator']");
    private readonly By addCollaboratorInput = By.XPath("//input[@data-ref='collaborator-input']");
    private readonly By unlockNoteButton = By.XPath("//button[@data-ref='unlock-note']");
    private readonly By deleteCollaboratorButton = By.XPath("//button[@data-ref='delete-collaborator']");
    private readonly By toastMessage = By.ClassName("blazored-toast-message");
    private readonly By toastCloseButton = By.ClassName("blazored-toast-close-icon");
    private readonly By waitMessage = By.XPath("//h3[@data-ref='wait-message']");

    public EditNotePage(IWebDriver driver)
    {
        this.driver = driver;
    }

    public EditNotePage InsertNoteContent(string content)
    {
        driver.FindElement(noteContentInput).SendKeys(content);

        return this;
    }

    public SaveNoteModal ClickSaveButton()
    {
        driver.FindElement(saveButton).Click();

        return new SaveNoteModal(driver);
    }

    public string AddCollaboratorAndGetNotification(string email)
    {
        driver.WaitUntilElementExists(addCollaboratorButton);
        driver.FindElement(addCollaboratorInput).SendKeys(email);
        CloseToastMessageIfExists();

        Policy
            .Handle<ElementClickInterceptedException>()
            .WaitAndRetry(retryCount: 10, sleepDurationProvider: _ => TimeSpan.FromSeconds(1))
            .Execute(() => driver.FindElement(addCollaboratorButton).Click());
        
        driver.WaitUntilElementExists(toastMessage);
        var notificationContent = Policy
            .HandleResult<string>(x => x.IsNullOrWhiteSpace())
            .WaitAndRetry(retryCount: 10, sleepDurationProvider: _ => TimeSpan.FromSeconds(1))
            .Execute(() => driver.FindElement(toastMessage).Text);

        return notificationContent;
    }
    
    public string ClickUnlockNoteAndGetNotification()
    {
        var button = driver.WaitUntilElementExists(unlockNoteButton);
        var js = (IJavaScriptExecutor)driver;
        js.ExecuteScript("arguments[0].click();", button);

        driver.WaitUntilElementExists(toastMessage);
        var notificationContent = Policy
            .HandleResult<string>(x => x.IsNullOrWhiteSpace())
            .WaitAndRetry(retryCount: 10, sleepDurationProvider: _ => TimeSpan.FromSeconds(1))
            .Execute(() => driver.FindElement(toastMessage).Text);

        return notificationContent;
    }

    public string DeleteCollaboratorFromSharedNote()
    {
        var button = driver.WaitUntilElementExists(deleteCollaboratorButton);
        var js = (IJavaScriptExecutor)driver;
        js.ExecuteScript("arguments[0].click();", button);

        driver.WaitUntilElementExists(toastMessage);
        var notificationContent = Policy
            .HandleResult<string>(x => x.IsNullOrWhiteSpace())
            .WaitAndRetry(retryCount: 10, sleepDurationProvider: _ => TimeSpan.FromSeconds(1))
            .Execute(() => driver.FindElement(toastMessage).Text);

        return notificationContent;
    }

    public string GetWaitMessage()
        => driver.WaitUntilElementExists(waitMessage).Text;

    private void CloseToastMessageIfExists()
    {
        try
        {
            driver.FindElement(toastCloseButton).Click();
        } 
        catch (NoSuchElementException)
        {
            return;
        }
        
    }
}
