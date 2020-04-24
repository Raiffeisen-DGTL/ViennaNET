using ViennaNET.Mediator.Seedwork;
using ViennaNET.Orm.DefaultConfiguration;
using ViennaNET.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace ViennaNET.Orm.Tests.Unit
{
  [TestFixture, Category("Unit"), TestOf(typeof(OrmPackage))]
  public class OrmPackageTests
  {
    Container _container;

    [OneTimeSetUp]
    public void TestInit()
    {
      _container = new Container();
    }

    [Test]
    public void InstallTest()
    {
      Assert.That(() =>
      {
        _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        _container.RegisterInstance(new Mock<IEventCollectorFactory>().Object);
        _container.RegisterInstance(new Mock<ISecurityContextFactory>().Object);
        var configuration = new ConfigurationBuilder().AddJsonFile("TestConfiguration/orm.json", optional: true)
                                                      .Build();
        _container.RegisterInstance<IConfiguration>(configuration);
        _container.RegisterPackages(new[] { typeof(OrmPackage).Assembly });
        _container.Verify();

      }, Throws.Nothing);
    }
  }
}