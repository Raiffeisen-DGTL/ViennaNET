using Microsoft.Extensions.Configuration;
using NHibernate;
using ViennaNET.Orm.Configuration;
using ViennaNET.Orm.Factories;
using ViennaNET.Utils;

namespace ViennaNET.Orm.SQLite
{
  public class SQLiteSessionFactoryProviderGetter : ISessionFactoryProviderGetter
  {
    private readonly IConfiguration _configuration;
    private readonly IInterceptor _interceptor;

    /// <summary>
    /// Инициализирует провайдер ссылками на <see cref="IConfiguration"/> и <see cref="IInterceptor"/>
    /// </summary>
    /// <param name="configuration">Ссылка на интерфейс, предоставляющий доступ к конфигурации</param>
    /// <param name="interceptor">Ссылка на интерфейс перехватчика для сущностей NHibernate</param>
    public SQLiteSessionFactoryProviderGetter(IConfiguration configuration, IInterceptor interceptor)
    {
      _configuration = configuration.ThrowIfNull(nameof(configuration));
      _interceptor = interceptor.ThrowIfNull(nameof(interceptor));
    }

    /// <inheritdoc />
    public string Type => "SQLite";

    /// <inheritdoc />
    public ISessionFactoryProvider GetSessionFactoryProvider(ConnectionInfo info)
    {
      return new SQLiteSessionFactoryProvider(info.Nick, _configuration, _interceptor);
    }
  }
}