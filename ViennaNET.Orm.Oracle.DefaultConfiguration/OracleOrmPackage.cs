using ViennaNET.Orm.Factories;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace ViennaNET.Orm.Oracle.DefaultConfiguration
{
  /// <inheritdoc />
  /// <summary>
  /// Пакет SimpleInjector для работы с БД Oracle
  /// </summary>
  public class OracleOrmPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      container.Collection.Append<ISessionFactoryProviderGetter, OracleSessionFactoryProviderGetter>(Lifestyle.Singleton);
    }
  }
}
