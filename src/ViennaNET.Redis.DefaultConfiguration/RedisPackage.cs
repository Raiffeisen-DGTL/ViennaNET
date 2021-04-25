using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Diagnostic;
using ViennaNET.Redis.Diagnostic;
using ViennaNET.Redis.Implementation.Default;
using ViennaNET.Redis.Implementation.Silent;

namespace ViennaNET.Redis.DefaultConfiguration
{
  /// <inheritdoc />
  /// <summary>
  /// Пакет SimpleInjector для работы с Redis
  /// </summary>
  public class RedisPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      container.Register<IRedisDatabaseProvider, RedisDatabaseProvider>(Lifestyle.Singleton);
      container.Register<ISilentRedisDatabaseProvider, SilentRedisDatabaseProvider>(Lifestyle.Singleton);
      container.Register<IConnectionConfiguration, ConnectionConfiguration>(Lifestyle.Singleton);
      container.Register<ICacheService, CacheService>(Lifestyle.Singleton);
      container.Register<ISilentCacheService, SilentCacheService>(Lifestyle.Singleton);

      container.Collection.Append<IDiagnosticImplementor, RedisConnectionChecker>(Lifestyle.Singleton);
    }
  }
}
