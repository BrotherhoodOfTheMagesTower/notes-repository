using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using SeleniumTests.Constants;
using SeleniumTests.Infrastructure.Builders;

namespace SeleniumTests.Fixtures;

public class BaseRemoteFixture : IDisposable
{
    public const string CollectionName = "Tests on basic remote fixture";

    public RemoteWebDriver WebDriver { get; }

    public BaseRemoteFixture()
	{
        WebDriver = new RemoteWebDriver(new Uri(Urls.seleniumHub), new ChromeOptions());
    }
    
    public void Dispose()
    {
        // ... clean up test data from the database ...
    }
}
