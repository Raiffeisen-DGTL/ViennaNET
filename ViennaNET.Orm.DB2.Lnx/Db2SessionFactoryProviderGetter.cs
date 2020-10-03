using ViennaNET.Orm.Configuration;
using ViennaNET.Orm.Factories;
using ViennaNET.Utils;
using Microsoft.Extensions.Configuration;
using NHibernate;

namespace ViennaNET.Orm.DB2
{
  /// <inheritdoc />
  public class Db2SessionFactoryProviderGetter : ISessionFactoryProviderGetter
  {
    private readonly IConfiguration _configuration;
    private readonly IInterceptor _interceptor;

    /// <summary>
    /// Инициализирует провайдер ссылками на <see cref="IConfiguration"/> и <see cref="IInterceptor"/>
    /// </summary>
    /// <param name="configuration">Ссылка на интерфейс, предоставляющий доступ к конфигурации</param>
    /// <param name="interceptor">Ссылка на интерфейс перехватчика для сущностей NHibernate</param>
    public Db2SessionFactoryProviderGetter(IConfiguration configuration, IInterceptor interceptor)
    {
      _configuration = configuration.ThrowIfNull(nameof(configuration));
      _interceptor = interceptor.ThrowIfNull(nameof(interceptor));
    }

    /// <inheritdoc />
    public string Type => "DB2";

    /// <inheritdoc />
    public ISessionFactoryProvider GetSessionFactoryProvider(ConnectionInfo info)
    {
      return new Db2SessionFactoryProvider(info.Nick, _configuration, _interceptor);
    }
  }
}
