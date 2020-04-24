using ViennaNET.Orm.Factories;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace ViennaNET.Orm.MSSQL.DefaultConfiguration
{
  /// <inheritdoc />
  /// <summary>
  /// Пакет SimpleInjector для работы с БД MS SQL
  /// </summary>
  public class MsSqlOrmPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      container.Collection.Append<ISessionFactoryProviderGetter, MsSqlSessionFactoryProviderGetter>(Lifestyle.Singleton);
    }
  }
}
