using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace SeleniumTests.Fixtures;

[CollectionDefinition(CollectionName)]
public class BaseRemoteFixture : ICollectionFixture<BaseRemoteFixture>
{
    public const string CollectionName = "Tests on basic remote fixture";

    public RemoteWebDriver WebDriver { get; }

    public BaseRemoteFixture()
	{
        WebDriver = new RemoteWebDriver(new Uri("http://localhost:4445"), new ChromeOptions());
    }
}
