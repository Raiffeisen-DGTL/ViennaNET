using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Orm.Factories;

namespace ViennaNET.Orm.SQLite.DefaultConfiguration
{
  public class SQLiteOrmPackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.Collection.Append<ISessionFactoryProviderGetter, SQLiteSessionFactoryProviderGetter>(Lifestyle.Singleton);
    }
  }
}