using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Factories;
using ViennaNET.Orm.Seedwork;
using Moq;
using NUnit.Framework;

namespace ViennaNET.Orm.Tests.Unit.Factories
{
  [TestFixture, Category("Unit"), TestOf(typeof(ApplicationContextProvider))]
  public class ApplicationContextProviderTests
  {
    [Test]
    public void GetNick_EntityWithCustomNick_NickCorrectlyGot()
    {
      var appContext = GetApplicationContextProvider(new TestContext(), out var sessionFactoryProvider);

      var nick = appContext.GetNick(typeof(TestCommand));

      Assert.Multiple(() =>
      {
        Assert.That(nick == "mssql");
        sessionFactoryProvider.Verify(x => x.AddClass(typeof(TestCommand), null));
      });
    }

    private static ApplicationContextProvider GetApplicationContextProvider(TestContext context, out Mock<ISessionFactoryProvider> sessionFactoryProvider)
    {
      sessionFactoryProvider = new Mock<ISessionFactoryProvider>();
      var sessionFactoryProvidersManager = new Mock<ISessionFactoryProvidersManager>();
      sessionFactoryProvidersManager.Setup(x => x.GetSessionFactoryProvider(It.IsAny<string>()))
                                    .Returns(sessionFactoryProvider.Object);
      var appContext = new ApplicationContextProvider(new[] { context }, sessionFactoryProvidersManager.Object);
      return appContext;
    }

    [Test]
    public void GetNick_EntityWithDefaultNick_NickCorrectlyGot()
    {
      var appContext = GetApplicationContextProvider(new TestContext(false, true), out _);

      var nick = appContext.GetNick(typeof(TestCommand));

      Assert.That(nick == "default");
    }

    [Test]
    public void GetNick_NoEntityInContext_Exception()
    {
      var appContext = GetApplicationContextProvider(new TestContext(), out _);

      Assert.Throws<EntityRepositoryException>(() => appContext.GetNick(typeof(object)), "Entity Object is not registered in factory");
    }

    [Test]
    public void GetNickForCommand_NoCommandInContext_Exception()
    {
      var appContext = GetApplicationContextProvider(new TestContext(), out _);

      Assert.Throws<EntityRepositoryException>(() => appContext.GetNickForCommand(typeof(object)), "Command Object is not registered in factory");
    }

    [Test]
    public void GetNickForNamedQuery_NoQueryInContext_Exception()
    {
      var appContext = GetApplicationContextProvider(new TestContext(), out _);

      Assert.Throws<EntityRepositoryException>(() => appContext.GetNickForNamedQuery("object"), "Named query object is not registered in factory");
    }

    [Test]
    public void GetNickForCommand_CommandWithCustomNick_NickCorrectlyGot()
    {
      var appContext = GetApplicationContextProvider(new TestContext(), out _);

      var nick = appContext.GetNickForCommand(typeof(TestCommand));

      Assert.That(nick == "db2");
    }

    [Test]
    public void GetNickForCommand_CommandWithDefaultNick_NickCorrectlyGot()
    {
      var appContext = GetApplicationContextProvider(new TestContext(false, true), out _);

      var nick = appContext.GetNickForCommand(typeof(TestCommand));

      Assert.That(nick == "default");
    }

    [Test]
    public void GetNickForNamedQuery_QueryWithCustomNick_NickCorrectlyGot()
    {
      var appContext = GetApplicationContextProvider(new TestContext(), out _);

      var nick = appContext.GetNickForNamedQuery("testQuery");

      Assert.That(nick == "oracle");
    }

    [Test]
    public void GetNickForNamedQuery_QueryWithDefaultNick_NickCorrectlyGot()
    {
      var appContext = GetApplicationContextProvider(new TestContext(false, true), out _);

      var nick = appContext.GetNickForNamedQuery("testQuery");

      Assert.That(nick == "default");
    }

    [Test]
    public void Ctor_EntityRegisteredTwiceAsEntityAndValueObject_ExceptionThrown()
    {
      Assert.Throws<EntityRepositoryException>(() => GetApplicationContextProvider(new TestContext(true), out _),
                                               "Entity TestCommand is not registered in factory");
    }

    [Test]
    public void Ctor_EntityNotImplementedIEntityKeyInterface_ExceptionThrown()
    {
      Assert.Throws<EntityRegistrationException>(() => GetApplicationContextProvider(new TestContext(true, false, true), out _),
                                               "All entities should implement IEntityKey interface. The entity of type BadEntity does not implement it.");
    }
  }
}
