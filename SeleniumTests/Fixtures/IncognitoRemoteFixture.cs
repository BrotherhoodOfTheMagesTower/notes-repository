using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using SeleniumTests.Constants;
using SeleniumTests.Infrastructure.Seeders;

namespace SeleniumTests.Fixtures;

public class IncognitoRemoteFixture : IAsyncLifetime
{
    public IWebDriver WebDriver { get; }
    public IWebDriver IncognitoWebDriver { get; }
    public BasicSeedingTask BasicSeedingTask { get; private set; }
    public BasicSeedingReport BasicSeedingReport { get; private set; }

    public IncognitoRemoteFixture()
    {
        WebDriver = new RemoteWebDriver(new Uri(Urls.seleniumHub), new ChromeOptions());
        var options = new ChromeOptions();
        options.AddArgument("incognito");
        IncognitoWebDriver = new RemoteWebDriver(new Uri(Urls.seleniumHub), options);
    }

    public async Task InitializeAsync()
    {
        BasicSeedingTask = new BasicSeedingTask(
            accountsCount: 2,
            notesPerAccountCount: 1,
            directoriesPerAccountCount: 1,
            eventsPerAccountCount: 1,
            imagesPerAccountCount: 1,
            createCollaborators: true);
        BasicSeedingReport = await BasicSeeder.CreateEnvironment(BasicSeedingTask);
    }

    public async Task DisposeAsync()
    {
        await BasicSeeder.CleanEnvironment(BasicSeedingReport);
        WebDriver.Quit();
        IncognitoWebDriver.Quit();
    }
}
