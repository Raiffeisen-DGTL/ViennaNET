using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Mediator.DefaultConfiguration;
using ViennaNET.Orm.DefaultConfiguration;
using ViennaNET.Orm.Seedwork;
using ViennaNET.Orm.SQLite.DefaultConfiguration;
using ViennaNET.SimpleInjector.Extensions;

namespace OrmService
{
  public class OrmServicePackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.Collection.Append<IBoundedContext, DbContext>(Lifestyle.Singleton);

      container.AddPackage(new MediatorPackage())
               .AddPackage(new OrmPackage())
               .AddPackage(new SQLiteOrmPackage());
    }
  }
}
