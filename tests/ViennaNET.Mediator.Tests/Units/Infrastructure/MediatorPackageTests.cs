using NUnit.Framework;
using SimpleInjector;
using ViennaNET.Mediator.DefaultConfiguration;
using ViennaNET.Validation.Behaviors;
using ViennaNET.Validation.Rules;

namespace ViennaNET.Mediator.Tests.Units.Infrastructure
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(MediatorPackage))]
  public class MediatorPackageTests
  {
    [OneTimeSetUp]
    public void TestInit()
    {
      _container = new Container();
    }

    private Container _container;

    [Test]
    public void InstallTest()
    {
      Assert.That(() =>
      {
        _container.Collection.Register(typeof(IMessageValidation), GetType().Assembly);
        _container.RegisterPackages(new[] { typeof(MediatorPackage).Assembly });
        _container.RegisterBroadcastPreProcessor<MessageValidationBehavior>(-1);
        _container.Verify();
      }, Throws.Nothing);
    }
  }
}