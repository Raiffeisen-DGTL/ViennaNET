using ViennaNET.Redis.DefaultConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SimpleInjector;
using ViennaNET.Redis.Implementation.Default;

namespace ViennaNET.Redis.Tests.Unit
{
  [TestFixture, Category("Unit"), TestOf(typeof(RedisPackage))]
  public class RedisPackageTests
  {
    Container _container;

    [OneTimeSetUp]
    public void TestInit()
    {
      _container = new Container();
    }

    [Test]
    public void Verify_PackageIsCorrect_NoException()
    {
      Assert.That(() =>
      {
        var configuration = new ConfigurationBuilder().AddJsonFile("TestConfiguration/redis.json", optional: true)
                                                      .Build();
        _container.RegisterInstance<IConfiguration>(configuration);
        _container.RegisterInstance<ILoggerFactory>(new NullLoggerFactory());
        _container.Register(typeof(ILogger<>), typeof(NullLogger<>), Lifestyle.Singleton);
        _container.RegisterPackages(new[] { typeof(RedisPackage).Assembly });
        _container.Verify();

      }, Throws.Nothing);
    }
  }
}