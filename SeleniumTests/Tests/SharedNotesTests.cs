using FluentAssertions;
using OpenQA.Selenium;
using SeleniumTests.Constants;
using SeleniumTests.Extensions;
using SeleniumTests.Fixtures;
using SeleniumTests.Infrastructure.Seeders;

namespace SeleniumTests.Tests;

public class SharedNotesTests : IClassFixture<BaseRemoteFixture>
{
    private readonly IWebDriver driver;
    private BasicSeedingReport report;

    public SharedNotesTests(BaseRemoteFixture fixture)
    {
        driver = fixture.WebDriver;
        report = fixture.BasicSeedingReport;
    }

    [Fact]
    public void UserIsNotAbleToShareNoteWithOtherUserThatDoesNotExist()
    {
        //GIVEN
        var firstUser = report.Users.ElementAt(3);
        var secondUser = "fake@mail.com";
        var noteContent = "Lorem Ipsum";
        var noteTitle = "Test 2";

        //WHEN
        var notification = driver
            .GoToLoginPage()
            .InsertEmail(firstUser.Name)
            .InsertPassword(SeederData.password)
            .Login()
            .ClickNewNoteFromNavMenu()
            .InsertNoteContent(noteContent)
            .ClickSaveButton()
            .InsertTitle(noteTitle)
            .ClickSave()
            .InsertIntoSearchBarAndClickResult(noteTitle)
            .AddCollaboratorAndGetNotification(secondUser);

        //THEN
        notification.Should().Be("User doesn't exists!");
    }
    
    [Fact]
    public void UserIsAbleToUnlockSharedNote()
    {
        //GIVEN
        var owner = report.Collaborators.First().Item1;

        //WHEN
        var editNotePage = driver
            .GoToLoginPage()
            .InsertEmail(owner)
            .InsertPassword(SeederData.password)
            .Login()
            .InsertIntoSearchBarAndClickResult(SeederData.sharedNoteTitle);

        //THEN
        editNotePage.ClickUnlockNoteAndGetNotification().Should().Be("Note has been correctly unlocked!");
    }
    
    [Fact]
    public void UserIsAbleToDisplayAllSharedNotes()
    {
        //GIVEN
        var owner = report.Collaborators.ElementAt(1).Item2;

        //WHEN
        var sharedNotesPage = driver
            .GoToLoginPage()
            .InsertEmail(owner)
            .InsertPassword(SeederData.password)
            .Login()
            .ClickSharedFromNavMenu();

        //THEN
        sharedNotesPage.GetAllVisibleSharedNoteTitles().Should().HaveCount(1).And.AllBe(SeederData.sharedNoteTitle);
        sharedNotesPage.GetAllVisibleSharedNoteContents().Should().HaveCount(1).And.AllBe(SeederData.sharedNoteContent);
    }

    [Fact]
    public void UserIsAbleDeleteCollaboratorFromSharedNote()
    {
        //GIVEN
        var owner = report.Collaborators.ElementAt(2).Item1;

        //WHEN
        var notification = driver
            .GoToLoginPage()
            .InsertEmail(owner)
            .InsertPassword(SeederData.password)
            .Login()
            .InsertIntoSearchBarAndClickResult(SeederData.sharedNoteTitle)
            .DeleteCollaboratorFromSharedNote();

        //THEN
        notification.Should().Be("Collaborator has been deleted");
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
            .ClickNewNoteFromNavMenu()
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
