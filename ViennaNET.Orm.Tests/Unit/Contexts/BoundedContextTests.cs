using System.Linq;
using System.Reflection;
using NUnit.Framework;
using ViennaNET.Orm.Seedwork;
using ViennaNET.Orm.Tests.Unit.DSL;

namespace ViennaNET.Orm.Tests.Unit.Contexts
{
  [TestFixture(Category = "Unit", TestOf = typeof(BoundedContext))]
  public class BoundedContextTests
  {
    [Test]
    public void AddEntityTest()
    {
      var nick = "nick";
      
      var context = Given.ApplicationContext.WithEntity(nick).Please();

      CollectionAssert.Contains(context.Entities, (typeof(TestEntity), nick, (Assembly)null));
    }

    [Test]
    public void AddAllEntitiesTest()
    {
      var nick = "nick";

      var context = Given.ApplicationContext.AddAllEntities(nick).Please();

      Assert.Multiple(() =>
      {
        CollectionAssert.Contains(context.Entities, (typeof(TestEntity), nick, (Assembly)null));
        Assert.IsFalse(context.Entities.Any(e => e.Item1 == typeof(BadEntityAbstract)));
        Assert.IsFalse(context.Entities.Any(e => e.Item1 == typeof(EntityBase)));
      });
    }

    [Test]
    public void AddBadEntityTest()
    {
      Assert.Throws<EntityRegistrationException>(() => Given.ApplicationContext.WithBadEntity().Please());
    }
  }
}
