using ViennaNET.Orm.Factories;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace ViennaNET.Orm.PostgreSql.DefaultConfiguration
{
  public class PostgreSqlOrmPackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.Collection.Append<ISessionFactoryProviderGetter, PostgreSqlSessionFactoryProviderGetter>(Lifestyle.Singleton);
    }
  }
}