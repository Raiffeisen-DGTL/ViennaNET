using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate;
using ViennaNET.Orm.Configuration;
using ViennaNET.Orm.Factories;
using ViennaNET.Utils;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ViennaNET.Orm.MSSQL
{
  /// <inheritdoc />
  public class MsSqlSessionFactoryProviderGetter : ISessionFactoryProviderGetter
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
    public MsSqlSessionFactoryProviderGetter(IConfiguration configuration, IInterceptor interceptor,
      ILoggerFactory loggerFactory)
    {
      _configuration = configuration.ThrowIfNull(nameof(configuration));
      _interceptor = interceptor.ThrowIfNull(nameof(interceptor));
      _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
    }

    /// <inheritdoc />
    public string Type => "MSSQL";

    /// <inheritdoc />
    public ISessionFactoryProvider GetSessionFactoryProvider(ConnectionInfo info)
    {
      return new MsSqlSessionFactoryProvider(info.Nick, _configuration, _interceptor, _loggerFactory,
        _loggerFactory.CreateLogger<MsSqlSessionFactoryProvider>());
    }
  }
}