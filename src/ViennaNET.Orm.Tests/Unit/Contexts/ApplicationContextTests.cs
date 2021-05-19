using System.Linq;
using System.Reflection;
using NUnit.Framework;
using ViennaNET.Orm.Tests.Unit.DSL;

namespace ViennaNET.Orm.Tests.Unit.Contexts
{
  [TestFixture(Category = "Unit", TestOf = typeof(ApplicationContext))]
  public class ApplicationContextTests
  {
    [Test]
    public void AddCommandTest()
    {
      var nick = "nick";

      var context = Given.ApplicationContext.WithCommand(nick).Please();

      CollectionAssert.Contains(context.Commands, (typeof(TestCommand), nick));
    }

    [Test]
    public void AddAllCommandsTest()
    {
      var nick = "nick";

      var context = Given.ApplicationContext.AddAllCommands(nick).Please();

      Assert.Multiple(() =>
      {
        CollectionAssert.Contains(context.Commands, (typeof(TestCommand), nick));
        Assert.IsFalse(context.Commands.Any(x => x.Item1 == typeof(BadCommand)));
      });
    }

    [Test]
    public void AddNamedQueryTest()
    {
      var nick = "nick";
      var queryName = "queryName";

      var context = Given.ApplicationContext.WithNamedQuery(queryName, nick).Please();

      CollectionAssert.Contains(context.NamedQueries, (queryName, nick));
    }

    [Test]
    public void AddCustomQueryTest()
    {
      var nick = "nick";

      var context = Given.ApplicationContext.WithQuery(nick).Please();

      CollectionAssert.Contains(context.CustomQueries, (typeof(TestEntity), nick));
    }

    [Test]
    public void AddIntegrationEventTest()
    {
      var nick = "nick";

      var context = Given.ApplicationContext.WithIntegrationEvent(nick).Please();

      CollectionAssert.Contains(context.IntegrationEvents, (typeof(TestIntegrationEvent), nick, (Assembly)null));
    }

    [Test]
    public void AddAllIntegrationEventsTest()
    {
      var nick = "nick";

      var context = Given.ApplicationContext.AddAllIntegrationEvents(nick).Please();

      Assert.Multiple(() =>
      {
        CollectionAssert.Contains(context.IntegrationEvents, (typeof(TestIntegrationEvent), nick, (Assembly)null));
        Assert.IsFalse(context.IntegrationEvents.Any(e => e.Item1 == typeof(BadIntegrationEvent)));
      });
    }
  }
}
