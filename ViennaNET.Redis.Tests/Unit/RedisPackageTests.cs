using ViennaNET.Redis.DefaultConfiguration;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SimpleInjector;

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
        _container.RegisterPackages(new[] { typeof(RedisPackage).Assembly });
        _container.Verify();

      }, Throws.Nothing);
    }
  }
}