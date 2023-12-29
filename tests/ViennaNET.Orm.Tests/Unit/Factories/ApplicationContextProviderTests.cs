using NUnit.Framework;
using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Factories;
using ViennaNET.Orm.Tests.Unit.DSL;

namespace ViennaNET.Orm.Tests.Unit.Factories;

[TestFixture]
[Category("Unit")]
[TestOf(typeof(ApplicationContextProvider))]
public class ApplicationContextProviderTests
{
    [Test]
    public void GetNick_EntityWithCustomNick_NickCorrectlyGot()
    {
        var nick = "nick";
        var appContext = Given.AppliationContextProvider(b => b.WithEntity(nick).Please());
        var nickReceived = appContext.GetNick(typeof(TestEntity));

        Assert.That(nickReceived, Is.EqualTo(nick));
    }

    [Test]
    public void GetNick_NoEntityInContext_Exception()
    {
        var appContext = Given.AppliationContextProvider();

        Assert.That(() => appContext.GetNick(typeof(object)),
            Throws.InstanceOf<EntityRepositoryException>()
                  .And.Message.EqualTo("Entity Object is not registered in factory"));
    }

    [Test]
    public void GetNickForCommand_NoCommandInContext_Exception()
    {
        var appContext = Given.AppliationContextProvider();

        Assert.That(() => appContext.GetNickForCommand(typeof(object)),
            Throws.InstanceOf<EntityRepositoryException>()
                  .And.Message.EqualTo("Command Object is not registered in factory"));

    }

    [Test]
    public void GetNickForNamedQuery_NoQueryInContext_Exception()
    {
        var appContext = Given.AppliationContextProvider();

        Assert.That(() => appContext.GetNickForNamedQuery("object"),
            Throws.InstanceOf<EntityRepositoryException>()
                  .And.Message.EqualTo("Named query object is not registered in factory"));
    }

    [Test]
    public void GetNickForCommand_CommandWithCustomNick_NickCorrectlyGot()
    {
        const string nick = "nick";
        var appContext = Given.AppliationContextProvider(b => b.WithCommand(nick).Please());
        var nickReceived = appContext.GetNickForCommand(typeof(TestCommand));

        Assert.That(nickReceived, Is.EqualTo(nick));
    }

    [Test]
    public void GetNickForNamedQuery_QueryWithCustomNick_NickCorrectlyGot()
    {
        const string nick = "nick";
        const string name = "name";

        var appContext = Given.AppliationContextProvider(b => b.WithNamedQuery(name, nick).Please());
        var nickReceived = appContext.GetNickForNamedQuery(name);

        Assert.That(nickReceived, Is.EqualTo(nick));
    }

    [Test]
    public void Ctor_EntityRegisteredTwiceAsEntityAndValueObject_ExceptionThrown()
    {
        Assert.That(() => Given.AppliationContextProvider(b => b.WithEntity().WithQuery().Please()),
            Throws.InstanceOf<EntityRepositoryException>());
    }
}