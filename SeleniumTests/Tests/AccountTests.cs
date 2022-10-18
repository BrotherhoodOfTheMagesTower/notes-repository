using SeleniumTests.Fixtures;
using SeleniumTests.Extensions;
using SeleniumTests.Infrastructure.Seeders;
using OpenQA.Selenium;
using FluentAssertions;

namespace SeleniumTests.Tests
{
    public class AccountTests : IClassFixture<BaseLocalFixture>, IDisposable
    {
        private readonly IWebDriver driver;
        private BasicSeedingReport report;
        private string email = "mir@kol.pl";


        public AccountTests(BaseLocalFixture fixture)
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
            var password = "MirKol123!";

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
    }
}