using FluentAssertions;
using OpenQA.Selenium;
using SeleniumTests.Constants;
using SeleniumTests.Extensions;
using SeleniumTests.Fixtures;
using SeleniumTests.Infrastructure.Seeders;

namespace SeleniumTests.Tests;

public class SharedNotesTests : IClassFixture<BaseLocalFixture>
{
    private readonly IWebDriver driver;
    private BasicSeedingReport report;

    public SharedNotesTests(BaseLocalFixture fixture)
    {
        driver = fixture.WebDriver;
        report = fixture.BasicSeedingReport;
    }

    [Fact]
    public void UserIsAbleToShareNoteWithOtherUser()
    {
        //GIVEN
        var firstUser = report.Users.ElementAt(0);
        var secondUser = report.Users.ElementAt(1);
        var noteContent = "Lorem Ipsum";
        var noteTitle = "Test 1";

        //WHEN
        var notification = driver
            .GoToLoginPage()
            .InsertEmail(firstUser.Name)
            .InsertPassword(SeederData.password)
            .Login()
            .ClickNewNoteButtonFromNavMenu()
            .InsertNoteContent(noteContent)
            .ClickSaveButton()
            .InsertTitle(noteTitle)
            .ClickSave()
            .InsertIntoSearchBarAndClickResult(noteTitle)
            .AddCollaboratorAndGetNotification(secondUser.Name);

        //THEN
        notification.Should().Be("Collaborator was sucessfully added!");
    }
}
