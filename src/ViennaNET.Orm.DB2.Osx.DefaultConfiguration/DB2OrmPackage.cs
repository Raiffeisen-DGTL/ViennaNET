using ViennaNET.Orm.Factories;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace ViennaNET.Orm.DB2.DefaultConfiguration
{
  /// <inheritdoc />
  /// <summary>
  /// Пакет SimpleInjector для работы с БД DB2
  /// </summary>
  public class Db2OrmPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      container.Collection.Append<ISessionFactoryProviderGetter, Db2SessionFactoryProviderGetter>(Lifestyle.Singleton);
    }
  }
}