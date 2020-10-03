using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Factories;
using ViennaNET.Orm.Seedwork;
using Moq;
using NUnit.Framework;
using ViennaNET.Orm.Tests.Unit.DSL;

namespace ViennaNET.Orm.Tests.Unit.Factories
{
  [TestFixture, Category("Unit"), TestOf(typeof(ApplicationContextProvider))]
  public class ApplicationContextProviderTests
  {
    [Test]
    public void GetNick_EntityWithCustomNick_NickCorrectlyGot()
    {
      var nick = "nick";
      var appContext = Given.AppliationContextProvider(b => b.WithEntity(nick).Please());

      var nickReceived = appContext.GetNick(typeof(TestEntity));

      Assert.AreEqual(nickReceived, nickReceived);
    }

    [Test]
    public void GetNick_NoEntityInContext_Exception()
    {
      var appContext = Given.AppliationContextProvider();

      Assert.Throws<EntityRepositoryException>(() => appContext.GetNick(typeof(object)), "Entity Object is not registered in factory");
    }

    [Test]
    public void GetNickForCommand_NoCommandInContext_Exception()
    {
      var appContext = Given.AppliationContextProvider();

      Assert.Throws<EntityRepositoryException>(() => appContext.GetNickForCommand(typeof(object)), "Command Object is not registered in factory");
    }

    [Test]
    public void GetNickForNamedQuery_NoQueryInContext_Exception()
    {
      var appContext = Given.AppliationContextProvider();

      Assert.Throws<EntityRepositoryException>(() => appContext.GetNickForNamedQuery("object"), "Named query object is not registered in factory");
    }

    [Test]
    public void GetNickForCommand_CommandWithCustomNick_NickCorrectlyGot()
    {
      var nick = "nick";
      var appContext = Given.AppliationContextProvider(b => b.WithCommand(nick).Please());

      var nickReceived = appContext.GetNickForCommand(typeof(TestCommand));

      Assert.AreEqual(nick, nickReceived);
    }

    [Test]
    public void GetNickForNamedQuery_QueryWithCustomNick_NickCorrectlyGot()
    {
      var nick = "nick";
      var name = "name";
      var appContext = Given.AppliationContextProvider(b => b.WithNamedQuery(name, nick).Please());

      var nickReceived = appContext.GetNickForNamedQuery(name);

      Assert.AreEqual(nick, nickReceived);
    }

    [Test]
    public void Ctor_EntityRegisteredTwiceAsEntityAndValueObject_ExceptionThrown()
    {
      Assert.Throws<EntityRepositoryException>(
        () => Given.AppliationContextProvider(b => b.WithEntity().WithQuery().Please()));
    }
  }
}
