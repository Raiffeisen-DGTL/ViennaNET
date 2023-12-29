using System.Reflection;
using NUnit.Framework;
using ViennaNET.Orm.Tests.Unit.DSL;

namespace ViennaNET.Orm.Tests.Unit.Contexts;

[TestFixture(Category = "Unit", TestOf = typeof(ApplicationContext))]
public class ApplicationContextTests
{
    [Test]
    public void AddCommandTest()
    {
        var nick = "nick";

        var context = Given.ApplicationContext.WithCommand(nick).Please();

        Assert.That(context.Commands, Has.Member((typeof(TestCommand), nick)));
    }

    [Test]
    public void AddAllCommandsTest()
    {
        const string nick = "nick";

        var context = Given.ApplicationContext.AddAllCommands(nick).Please();

        Assert.Multiple(() =>
        {
            Assert.That(context.Commands, Has.Member((typeof(TestCommand), nick)));
            Assert.That(context.Commands.Any(x => x.Item1 == typeof(BadCommand)), Is.False);
        });
    }

    [Test]
    public void AddNamedQueryTest()
    {
        const string nick = "nick";
        const string queryName = "queryName";

        var context = Given.ApplicationContext.WithNamedQuery(queryName, nick).Please();

        Assert.That(context.NamedQueries, Has.Member((queryName, nick)));
    }

    [Test]
    public void AddCustomQueryTest()
    {
        const string nick = "nick";

        var context = Given.ApplicationContext.WithQuery(nick).Please();

        Assert.That(context.CustomQueries, Has.Member((typeof(TestEntity), nick)));
    }

    [Test]
    public void AddIntegrationEventTest()
    {
        const string nick = "nick";

        var context = Given.ApplicationContext.WithIntegrationEvent(nick).Please();

        Assert.That(context.IntegrationEvents, Has.Member((typeof(TestIntegrationEvent), nick, (Assembly)null!)));
    }

    [Test]
    public void AddAllIntegrationEventsTest()
    {
        const string nick = "nick";

        var context = Given.ApplicationContext.AddAllIntegrationEvents(nick).Please();

        Assert.Multiple(() =>
        {
            Assert.That(context.IntegrationEvents, Has.Member((typeof(TestIntegrationEvent), nick, (Assembly)null!)));
            Assert.That(context.IntegrationEvents.Any(e => e.Item1 == typeof(BadIntegrationEvent)), Is.False);
        });
    }
}