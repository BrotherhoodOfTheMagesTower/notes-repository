using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Infrastructure.Seeders;

namespace SeleniumTests.Fixtures;

public class IncognitoLocalFixture : IAsyncLifetime
{
    public IWebDriver WebDriver { get; }
    public IWebDriver IncognitoWebDriver { get; }
    public BasicSeedingTask BasicSeedingTask { get; private set; }
    public BasicSeedingReport BasicSeedingReport { get; private set; }

    public IncognitoLocalFixture()
    {
        WebDriver = new ChromeDriver(new ChromeOptions());
        var options = new ChromeOptions();
        options.AddArgument("incognito");
        IncognitoWebDriver = new ChromeDriver(options);
    }

    public async Task InitializeAsync()
    {
        BasicSeedingTask = new BasicSeedingTask(
            accountsCount: 10,
            notesPerAccountCount: 1,
            directoriesPerAccountCount: 1,
            eventsPerAccountCount: 1,
            imagesPerAccountCount: 1,
            createCollaborators: false);
        BasicSeedingReport = await BasicSeeder.CreateEnvironment(BasicSeedingTask);
    }

    public async Task DisposeAsync()
    {
        await BasicSeeder.CleanEnvironment(BasicSeedingReport);
        WebDriver.Quit();
        IncognitoWebDriver.Quit();
    }
}
