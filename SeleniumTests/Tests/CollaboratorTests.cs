using FluentAssertions;
using OpenQA.Selenium;
using SeleniumTests.Constants;
using SeleniumTests.Extensions;
using SeleniumTests.Fixtures;
using SeleniumTests.Infrastructure.Seeders;

namespace SeleniumTests.Tests;

public class CollaboratorTests : IClassFixture<IncognitoRemoteFixture>
{
    private readonly IWebDriver driver;
    private readonly IWebDriver incognitoDriver;
    private BasicSeedingReport report;

    public CollaboratorTests(IncognitoRemoteFixture fixture)
    {
        driver = fixture.WebDriver;
        incognitoDriver = fixture.IncognitoWebDriver;
        report = fixture.BasicSeedingReport;
    }

    [Fact]
    public void UserIsNotAbleToEditNoteWhenOtherUserIsEditingIt()
    {
        //GIVEN
        var owner = report.Collaborators.First().Item1;
        var collaborator = report.Collaborators.First().Item2;

        //WHEN
        driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(owner)
            .Login()
            .InsertIntoSearchBarAndClickResult(SeederData.sharedNoteTitle);
        var message = incognitoDriver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(collaborator)
            .Login()
            .ClickSharedFromNavMenu()
            .ClickFirstSharedNote(waitForEditNotePage: false)
            .GetWaitMessage();

        //THEN
        message.Should().Be($"Wait for user {owner} to finish editing the note.");
    }
}
