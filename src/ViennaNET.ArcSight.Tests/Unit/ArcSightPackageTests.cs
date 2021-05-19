using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SimpleInjector;
using ViennaNET.ArcSight.DefaultConfiguration;

namespace ViennaNET.ArcSight.Tests.Unit
{
  [TestFixture, Category("Unit"), TestOf(typeof(ArcSightPackage))]
  public class ArcSightPackageTests
  {
    private Container _container;

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
        var configuration = new ConfigurationBuilder().AddJsonFile("TestConfiguration/arcSight.json", optional: true)
                                                          .Build();
        _container.RegisterInstance<IConfiguration>(configuration);
        _container.RegisterSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        _container.RegisterPackages(new[] { typeof(ArcSightPackage).Assembly });
        _container.Verify();

      }, Throws.Nothing);
    }
  }
}