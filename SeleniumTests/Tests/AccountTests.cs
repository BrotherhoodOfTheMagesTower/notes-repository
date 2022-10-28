using SeleniumTests.Fixtures;
using SeleniumTests.Extensions;
using SeleniumTests.Infrastructure.Seeders;
using OpenQA.Selenium;
using FluentAssertions;

namespace SeleniumTests.Tests;

public class AccountTests : IClassFixture<BaseRemoteFixture>, IDisposable
{
    private readonly IWebDriver driver;
    private BasicSeedingReport report;
    private string email = "mir@kol.pl";
    private string password = "MirKol123!";

    public AccountTests(BaseRemoteFixture fixture)
    {
        driver = fixture.WebDriver;
        report = fixture.BasicSeedingReport;
    }

    public async void Dispose() => await BasicSeeder.RemoveUser(email);

    [Fact]
    public void UserIsAbleToRegisterWithFirstAndLastName()
    {
        //GIVEN
        var firstName = "Miros³aw";
        var lastName = "Kowalski";

        //WHEN
        driver
            .GoToRegisterPage()
            .InsertFirstName(firstName)
            .InsertLastName(lastName)
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .Register();
        var page = driver.GoToAccountManagement();

        //THEN
        page.GetFirstName().Should().Be(firstName);
        page.GetLastName().Should().Be(lastName);
    }
    
    [Fact]
    public void UserIsAbleToRegisterWithoutFirstAndLastName()
    {
        //GIVEN
        var name = "unknown";

        //WHEN
        driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .Register();
        var page = driver.GoToAccountManagement();

        //THEN
        page.GetFirstName().Should().Be(name);
        page.GetLastName().Should().Be(name);
    }
    
    [Fact]
    public void UserIsNotAbleToRegisterWithEmailWhichWasAlreadyUsed()
    {
        //GIVEN
        var email = report.Users.First().Name;

        //WHEN
        var page = driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .RegisterAndStay();

        //THEN
        page.GetSummaryErrors().Should().Be($"Username '{email}' is already taken.");
    }
    
    [Fact]
    public void UserIsNotAbleToRegisterWithIncorrectConfirmationPassword()
    {
        //WHEN
        var page = driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password + "fake")
            .RegisterAndStay();

        //THEN
        page.GetConfirmationPasswordErrors().Should().Be("The password and confirmation password do not match.");
    }
    
    [Fact]
    public void UserIsNotAbleToRegisterWithIncorrectLengthOfPassword()
    {
        //GIVEN
        var password = "M123!";

        //WHEN
        var page = driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .RegisterAndStay();

        //THEN
        page.GetPasswordErrors().Should().Be("The Password must be at least 6 and at max 100 characters long.");
    }
    
    [Fact]
    public void UserIsNotAbleToRegisterWithPasswordThatDoesNotHaveUppercase()
    {
        //GIVEN
        var password = "mirkol123!";

        //WHEN
        var page = driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .RegisterAndStay();

        //THEN
        page.GetSummaryErrors().Should().Be("Passwords must have at least one uppercase ('A'-'Z').");
    }
    
    [Fact]
    public void UserIsNotAbleToRegisterWithPasswordThatDoesNotHaveNonAlphanumericCharacter()
    {
        //GIVEN
        var password = "MirKol123";

        //WHEN
        var page = driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .RegisterAndStay();

        //THEN
        page.GetSummaryErrors().Should().Be("Passwords must have at least one non alphanumeric character.");
    }
    
    [Fact]
    public void UserIsNotAbleToRegisterWithPasswordThatDoesNotHaveDigit()
    {
        //GIVEN
        var password = "MirKol!";

        //WHEN
        var page = driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .RegisterAndStay();

        //THEN
        page.GetSummaryErrors().Should().Be("Passwords must have at least one digit ('0'-'9').");
    }
    
    [Fact]
    public void UserIsNotAbleToRegisterWithIncorrectEmailAddress()
    {
        //GIVEN
        var email = "mirkol.pl";

        //WHEN
        var page = driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .RegisterAndStay();

        //THEN
        page.GetEmailErrors().Should().Be("The Email field is not a valid e-mail address.");
    }
    
    [Fact]
    public void UserIsAbleToAddPhoneNumber()
    {
        //GIVEN
        var phoneNumber = "123";

        //WHEN
        driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .Register();
        var validation = driver
            .GoToAccountManagement()
            .InsertPhoneNumber(phoneNumber)
            .Update()
            .GetUpdateStatus();

        //THEN
        validation.Should().Contain("Your profile has been updated");
    }
    
    [Fact]
    public void UserIsAbleToChangePassword()
    {
        //GIVEN
        var newPassword = "MirKolNew123!";

        //WHEN
        driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .Register();
        var validation = driver
            .GoToAccountManagement()
            .SelectPassword()
            .InsertNewPassword(password, newPassword)
            .Update()
            .GetUpdateStatus();

        //THEN
        validation.Should().Contain("Your password has been changed.");
    }
    
    [Fact]
    public void UserIsAbleToDeleteAccount()
    {
        //WHEN
        driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .Register();
        driver
            .GoToAccountManagement()
            .SelectPersonalData()
            .Delete()
            .InsertPassword(password)
            .CloseAccount();
        var validation = driver
            .GoToLoginPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .LoginAndStay()
            .GetSummaryErrors();

        //THEN
        validation.Should().Be("Invalid login attempt.");
    }
    
    [Fact]
    public void UserIsNotAbleToDeleteAccountWhenPasswordIsIncorrect()
    {
        //WHEN
        driver
            .GoToRegisterPage()
            .InsertEmail(email)
            .InsertPassword(password)
            .InsertConfirmedPassword(password)
            .Register();
        var validation = driver
            .GoToAccountManagement()
            .SelectPersonalData()
            .Delete()
            .InsertPassword(password + "fake")
            .CloseAccountAndStay()
            .GetSummaryErrors();

        //THEN
        validation.Should().Be("Incorrect password.");
    }
}