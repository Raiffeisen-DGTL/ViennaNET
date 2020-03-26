using NUnit.Framework;
using StackExchange.Redis;

namespace ViennaNET.Redis.Tests.Unit
{
  [TestFixture(Category = "Unit", TestOf = typeof(ConnectionOptions))]
  public class ConnectionOptionsTests
  {
    [Test]
    public void GetConfigurationOptions_CorrectlyFilled()
    {
      var configuration = new ConnectionOptions(new ConfigurationOptions(), "9050", 5000, 10000, null);

      var options = configuration.GetConfigurationOptions();

      Assert.That(configuration.Key == "9050");
      Assert.That(options != null);
      Assert.That(options.ReconnectRetryPolicy.GetType() == typeof(ExponentialRetry));
    }
  }
}
