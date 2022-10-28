using OpenQA.Selenium;
using Polly;
using SeleniumTests.Infrastructure;
using SeleniumTests.PageObjects.Modals;

namespace SeleniumTests.Pages.Note;

public class EditNotePage
{
    private IWebDriver driver;
    private readonly By noteContentInput = By.XPath("//textarea[@data-ref='note-input']");
    private readonly By saveButton = By.XPath("//button[@data-ref='save-note']");
    private readonly By addCollaboratorInput = By.XPath("//input[@data-ref='collaborator-input']");
    private readonly By addCollaboratorButton = By.XPath("//button[@data-ref='add-collaborator']");
    private readonly By toastMessage = By.ClassName("blazored-toast-message");
    private readonly By toastCloseButton = By.ClassName("blazored-toast-close-icon");

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
        return driver.FindElement(toastMessage).Text;
    }

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
