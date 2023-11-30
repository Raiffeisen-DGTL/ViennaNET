using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate;
using ViennaNET.Orm.Configuration;
using ViennaNET.Orm.Factories;
using ViennaNET.Utils;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ViennaNET.Orm.SQLite
{
  public class SQLiteSessionFactoryProviderGetter : ISessionFactoryProviderGetter
  {
    private readonly IConfiguration _configuration;
    private readonly IInterceptor _interceptor;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    ///   Инициализирует провайдер ссылками на <see cref="IConfiguration" /> и <see cref="IInterceptor" />
    /// </summary>
    /// <param name="configuration">Ссылка на интерфейс, предоставляющий доступ к конфигурации</param>
    /// <param name="interceptor">Ссылка на интерфейс перехватчика для сущностей NHibernate</param>
    public SQLiteSessionFactoryProviderGetter(IConfiguration configuration, IInterceptor interceptor,
      ILoggerFactory loggerFactory)
    {
      _configuration = configuration.ThrowIfNull(nameof(configuration));
      _interceptor = interceptor.ThrowIfNull(nameof(interceptor));
      _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
    }

    /// <inheritdoc />
    public string Type => "SQLite";

    /// <inheritdoc />
    public ISessionFactoryProvider GetSessionFactoryProvider(ConnectionInfo info)
    {
      return new SQLiteSessionFactoryProvider(info.Nick, _configuration, _interceptor, _loggerFactory,
        _loggerFactory.CreateLogger<SQLiteSessionFactoryProvider>());
    }
  }
}