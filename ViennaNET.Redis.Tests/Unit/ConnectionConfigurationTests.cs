using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace ViennaNET.Redis.Tests.Unit
{
  [TestFixture(Category = "Unit", TestOf = typeof(ConnectionConfiguration))]
  public class ConnectionConfigurationTests
  {
    [Test]
    public void GetConnectionOptions_CorrectConnString_Parsed()
    {
      var configurationRoot = new ConfigurationBuilder()
        .AddJsonFile("TestConfiguration/redis.json", optional: true)
        .Build();

      var configuration = new ConnectionConfiguration(configurationRoot);

      var options = configuration.GetConnectionConfigurationOptions();

      var configurationOptions = options.GetConfigurationOptions();
      Assert.That(configurationOptions.EndPoints.Count == 2);
      Assert.That(configurationOptions.AllowAdmin);
    }

    [Test]
    public void GetConnectionOptions_EmptyConnString_ArgumentException()
    {
      var configurationRoot = new ConfigurationBuilder()
                              .AddJsonFile("TestConfiguration/redisNoConnection.json", optional: true)
                              .Build();
      var configuration = new ConnectionConfiguration(configurationRoot);

      Assert.That(() =>
      {
        configuration.GetConnectionConfigurationOptions();
      }, Throws.ArgumentException);
    }

    [Test]
    public void GetConnectionOptions_KeyLifetimePassed_Parsed()
    {
      var configurationRoot = new ConfigurationBuilder()
                              .AddJsonFile("TestConfiguration/redis.json", optional: true)
                              .Build();

      var configuration = new ConnectionConfiguration(configurationRoot);

      var options = configuration.GetConnectionConfigurationOptions();

      Assert.That(options.KeyLifetimes.Count == 2);
    }
  }
}
