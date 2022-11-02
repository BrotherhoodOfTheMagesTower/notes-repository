using FluentAssertions;
using OpenQA.Selenium;
using SeleniumTests.Constants;
using SeleniumTests.Extensions;
using SeleniumTests.Fixtures;
using SeleniumTests.Infrastructure.Seeders;

namespace SeleniumTests.Tests;

public class CalendarTests : IClassFixture<BaseRemoteFixture>
{
    private IWebDriver driver;
    private BasicSeedingReport report;

    public CalendarTests(BaseRemoteFixture fixture)
    {
        driver = fixture.WebDriver;
        report = fixture.BasicSeedingReport;
    }

    [Fact]
    public async void UserIsAbleToAddEventWithoutNote()
    {
        //GIVEN
        var email = report.Users.First().Name;
        var eventTitle = "Test1";
        var startAtTime = $"{DateTime.Now.Month}/{DateTime.Now.Day}/{DateTime.Now.Year} 23:59:00";

        //WHEN
        var notification = await driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu()
            .ClickOnCurrentDay()
            .WithStartAt(startAtTime)
            .WithTitle(eventTitle)
            .SaveAndGetNotification();

        //THEN
        notification.Should().Be("You have successfully added an event");
    }

    [Fact]
    public async void UserIsAbleToAddEventWithAttachedNote()
    {
        //GIVEN
        var email = report.Users.ElementAt(1).Name;
        var eventTitle = "Test1";
        var startAtTime = $"{DateTime.Now.Month}/{DateTime.Now.Day}/{DateTime.Now.Year} 23:59:00";

        //WHEN
        var calendarPage = driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu();
        var notification = await calendarPage
            .ClickOnCurrentDay()
            .WithStartAt(startAtTime)
            .WithTitle(eventTitle)
            .WithNote(SeederData.NoteTitle(1))
            .SaveAndGetNotification();

        //THEN
        notification.Should().Be("You have successfully added an event");
        calendarPage.ClickOnEvent(eventTitle).GetSelectedNoteTitle().Should().Be(SeederData.NoteTitle(1));
    }

    [Fact]
    public async void UserIsNotAbleToAddEventForAPastDate()
    {
        //GIVEN
        var email = report.Users.ElementAt(2).Name;
        var eventTitle = "Test1";
        var startAtTime = $"{DateTime.Now.Month}/{DateTime.Now.AddDays(-1).Day}/{DateTime.Now.Year} 12:22:00";

        //WHEN
        var notification = await driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu()
            .ClickOnCurrentDay()
            .WithTitle(eventTitle)
            .WithStartAt(startAtTime)
            .SaveAndGetNotification();

        //THEN
        notification.Should().Be("Something went wrong");
    }

    [Fact]
    public async void UserIsNotAbleToAddEventWhenEndAtDateIsLowerThanStartAtDate()
    {
        //GIVEN
        var email = report.Users.ElementAt(3).Name;
        var eventTitle = "Test1";
        var startAtTime = $"{DateTime.Now.Month}/{DateTime.Now.Day}/{DateTime.Now.Year} 23:59:00";
        var endAtTime = $"{DateTime.Now.AddMonths(-1).Month}/{DateTime.Now.AddDays(-3).Day}/{DateTime.Now.Year} 12:22:00";

        //WHEN
        var notification = await driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu()
            .ClickOnCurrentDay()
            .WithStartAt(startAtTime)
            .WithEndAt(endAtTime)
            .WithTitle(eventTitle)
            .SaveAndGetNotification();

        //THEN
        notification.Should().Be("Something went wrong");
    }

    [Fact]
    public async Task UserIsNotAbleToAddEventWithoutDeclaringEventTitle()
    {
        //GIVEN
        var email = report.Users.ElementAt(4).Name;

        //WHEN
        var errorMessage = (await driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu()
            .ClickOnCurrentDay()
            .Save())
            .GetTitleErrorMessage();

        //THEN
        errorMessage.Should().Be("Title is required.");
    }

    [Fact]
    public async void UserIsAbleToEditEventTitle()
    {
        //GIVEN
        var email = report.Users.ElementAt(5).Name;
        var newTitle = "ChangingTitle";

        //WHEN
        var notification = await driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu()
            .ClickOnEvent(SeederData.EventContent(1))
            .WithTitle(newTitle)
            .SaveAndGetNotification();

        //THEN
        notification.Should().Be("You have successfully edited an event");
    }

    [Fact]
    public async void UserIsAbleToEditEventStartAtDate()
    {
        //GIVEN
        var email = report.Users.ElementAt(6).Name;
        var shiftedTime = DateTime.Now.AddDays(17);
        var startAtTime = $"{shiftedTime.Month}/{shiftedTime.Day}/{shiftedTime.Year} 23:59:00";

        //WHEN
        var notification = await driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu()
            .ClickOnEvent(SeederData.EventContent(1))
            .WithStartAt(startAtTime)
            .SaveAndGetNotification();

        //THEN
        notification.Should().Be("You have successfully edited an event");
    }

    [Fact]
    public async void UserIsAbleToEditEventEndAtDate()
    {
        //GIVEN
        var email = report.Users.ElementAt(7).Name;
        var shiftedTime = DateTime.Now.AddDays(18);
        var endAtTime = $"{shiftedTime.Month}/{shiftedTime.Day}/{shiftedTime.Year} 23:59:00";

        //WHEN
        var notification = await driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu()
            .ClickOnEvent(SeederData.EventContent(1))
            .WithEndAt(endAtTime)
            .SaveAndGetNotification();

        //THEN
        notification.Should().Be("You have successfully edited an event");
    }

    [Fact]
    public async Task UserIsAbleToAttachNoteToEventAsync()
    {
        //GIVEN
        var email = report.Users.ElementAt(8).Name;

        //WHEN
        var calendarPage = driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu();
        var notification = await calendarPage
            .ClickOnEvent(SeederData.EventContent(1))
            .WithNote(SeederData.NoteTitle(1))
            .SaveAndGetNotification();
        var eventModal = calendarPage.ClickOnEvent(SeederData.EventContent(1));
        var selectedNoteTitle = eventModal.GetSelectedNoteTitle();

        //THEN
        notification.Should().Be("You have successfully edited an event");
        selectedNoteTitle.Should().Be(SeederData.NoteTitle(1));
        eventModal.GoToAttachedNote().GetNoteTitle().Should().Contain(SeederData.NoteTitle(1).ToUpper());
    }

    [Fact]
    public async void UserIsAbleToRemoveNoteFromEvent()
    {
        //GIVEN
        var email = report.Users.ElementAt(9).Name;

        //WHEN
        var calendarPage = await driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu()
            .ClickOnEvent(SeederData.EventContent(1))
            .WithNote(SeederData.NoteTitle(1))
            .Save();
        var notification = await calendarPage
            .ClickOnEvent(SeederData.EventContent(1))
            .WithNote("---")
            .SaveAndGetNotification();

        //THEN
        notification.Should().Be("You have successfully edited an event");
        calendarPage.ClickOnEvent(SeederData.EventContent(1)).GetSelectedNoteTitle().Should().Be("---");
    }

    [Fact]
    public void UserIsAbleToRemoveEvent()
    {
        //GIVEN
        var email = report.Users.ElementAt(10).Name;

        //WHEN
        var calendarPage = driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu();
        var notification = calendarPage
            .ClickOnEvent(SeederData.EventContent(1))
            .DeleteEventAndGetNotification();

        //THEN
        notification.Should().Be("You have successfully deleted an event");
        try { calendarPage.ClickOnEvent(SeederData.EventContent(1)); }
        catch (NoSuchElementException e) { e.Message.Should().Contain("no such element", because: "event was deleted"); }
    }

    [Fact]
    public async void UserIsAbleToRedirectFromEventToAttachedNote()
    {
        //GIVEN
        var email = report.Users.ElementAt(11).Name;

        //WHEN
        var calendarPage = driver
            .GoToLoginPage()
            .InsertPassword(SeederData.password)
            .InsertEmail(email)
            .Login()
            .ClickCalendarFromNavMenu();
        (await calendarPage
            .ClickOnEvent(SeederData.EventContent(1))
            .WithNote(SeederData.NoteTitle(1))
            .Save())
            .ClickOnEvent(SeederData.EventContent(1))
            .GoToAttachedNote()
            .GetNoteTitle()
        //THEN
            .Should().Contain(SeederData.NoteTitle(1).ToUpper());
    }
}
