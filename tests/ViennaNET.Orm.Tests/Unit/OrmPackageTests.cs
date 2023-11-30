using ViennaNET.Mediator.Seedwork;
using ViennaNET.Orm.DefaultConfiguration;
using ViennaNET.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace ViennaNET.Orm.Tests.Unit
{
  [TestFixture, Category("Unit"), TestOf(typeof(OrmPackage))]
  public class OrmPackageTests
  {
    [OneTimeSetUp]
    public void TestInit()
    {
      _container = new Container();
    }

    Container _container;

    [Test]
    public void InstallTest()
    {
      Assert.That(() =>
      {
        _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        _container.RegisterInstance(new Mock<IEventCollectorFactory>().Object);
        _container.RegisterInstance(new Mock<ISecurityContextFactory>().Object);
        _container.RegisterInstance(new Mock<ILoggerFactory>().Object);
        _container.Register(typeof(ILogger<>), typeof(NullLogger<>), Lifestyle.Singleton);

        var configuration = new ConfigurationBuilder().AddJsonFile("TestConfiguration/orm.json", optional: true)
          .Build();
        _container.RegisterInstance<IConfiguration>(configuration);
        _container.RegisterPackages(new[] { typeof(OrmPackage).Assembly });
        _container.Verify();
      }, Throws.Nothing);
    }
  }
}