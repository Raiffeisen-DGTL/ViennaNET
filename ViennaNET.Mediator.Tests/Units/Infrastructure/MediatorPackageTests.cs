using ViennaNET.Mediator.DefaultConfiguration;
using ViennaNET.Validation.Behaviors;
using ViennaNET.Validation.Rules;
using NUnit.Framework;
using SimpleInjector;

namespace ViennaNET.Mediator.Tests.Units.Infrastructure
{
  [TestFixture, Category("Unit"), TestOf(typeof(MediatorPackage))]
  public class MediatorPackageTests
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
        _container.Collection.Register(typeof(IMessageValidation), GetType().Assembly);
        _container.RegisterPackages(new [] { typeof(MediatorPackage).Assembly });
        _container.RegisterBroadcastPreProcessor<MessageValidationBehavior>(-1);
        _container.Verify();

      }, Throws.Nothing);
    }
  }
}