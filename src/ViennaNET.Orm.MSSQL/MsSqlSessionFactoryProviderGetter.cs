using Microsoft.Extensions.Configuration;
using NHibernate;
using ViennaNET.Orm.Configuration;
using ViennaNET.Orm.Factories;
using ViennaNET.Utils;

namespace ViennaNET.Orm.MSSQL
{
  /// <inheritdoc />
  public class MsSqlSessionFactoryProviderGetter : ISessionFactoryProviderGetter
  {
    private readonly IConfiguration _configuration;
    private readonly IInterceptor _interceptor;

    /// <summary>
    /// Инициализирует провайдер ссылками на <see cref="IConfiguration"/> и <see cref="IInterceptor"/>
    /// </summary>
    /// <param name="configuration">Ссылка на интерфейс, предоставляющий доступ к конфигурации</param>
    /// <param name="interceptor">Ссылка на интерфейс перехватчика для сущностей NHibernate</param>
    public MsSqlSessionFactoryProviderGetter(IConfiguration configuration, IInterceptor interceptor)
    {
      _configuration = configuration.ThrowIfNull(nameof(configuration));
      _interceptor = interceptor.ThrowIfNull(nameof(interceptor));
    }

    /// <inheritdoc />
    public string Type => "MSSQL";

    /// <inheritdoc />
    public ISessionFactoryProvider GetSessionFactoryProvider(ConnectionInfo info)
    {
      return new MsSqlSessionFactoryProvider(info.Nick, _configuration, _interceptor);
    }
  }
}
