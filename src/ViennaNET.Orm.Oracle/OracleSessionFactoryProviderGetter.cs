using ViennaNET.Orm.Configuration;
using ViennaNET.Orm.Factories;
using ViennaNET.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ViennaNET.Orm.Oracle
{
  /// <inheritdoc />
  public class OracleSessionFactoryProviderGetter : ISessionFactoryProviderGetter
  {
    private readonly IConfiguration _configuration;
    private readonly IInterceptor _interceptor;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    ///   Инициализирует провайдер ссылками на <see cref="IConfiguration" /> и <see cref="IInterceptor" />
    /// </summary>
    /// <param name="configuration">Ссылка на интерфейс, предоставляющий доступ к конфигурации</param>
    /// <param name="interceptor">Ссылка на интерфейс перехватчика для сущностей NHibernate</param>
    /// <param name="loggerFactory">Ссылка на стандартный интерфейс фабрики логгеров</param>
    public OracleSessionFactoryProviderGetter(IConfiguration configuration, IInterceptor interceptor,
      ILoggerFactory loggerFactory)
    {
      _configuration = configuration.ThrowIfNull(nameof(configuration));
      _interceptor = interceptor.ThrowIfNull(nameof(interceptor));
      _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
    }

    /// <inheritdoc />
    public string Type => "Oracle";

    /// <inheritdoc />
    public ISessionFactoryProvider GetSessionFactoryProvider(ConnectionInfo info)
    {
      return new OracleSessionFactoryProvider(info.Nick, _configuration, _interceptor, _loggerFactory,
        _loggerFactory.CreateLogger<OracleSessionFactoryProvider>());
    }
  }
}