using System.Reflection;
using NUnit.Framework;
using ViennaNET.Orm.Seedwork;
using ViennaNET.Orm.Tests.Unit.DSL;

namespace ViennaNET.Orm.Tests.Unit.Contexts;

[TestFixture(Category = "Unit", TestOf = typeof(BoundedContext))]
public class BoundedContextTests
{
    [Test]
    public void AddEntityTest()
    {
        const string nick = "nick";

        var context = Given.ApplicationContext.WithEntity(nick).Please();

        Assert.That(context.Entities, Has.Member((typeof(TestEntity), nick, (Assembly)null!)));
    }

    [Test]
    public void AddAllEntitiesTest()
    {
        var nick = "nick";

        var context = Given.ApplicationContext.AddAllEntities(nick).Please();

        Assert.Multiple(() =>
        {
            Assert.That(context.Entities, Has.Member((typeof(TestEntity), nick, (Assembly)null!)));
            Assert.That(context.Entities.Any(e => e.Item1 == typeof(BadEntityAbstract)), Is.False);
            Assert.That(context.Entities.Any(e => e.Item1 == typeof(EntityBase)), Is.False);
        });
    }

    [Test]
    public void AddBadEntityTest()
    {
        Assert.That(() => Given.ApplicationContext.WithBadEntity().Please(), Throws.InstanceOf<EntityRegistrationException>());
    }
}