using System;
using System.Collections.Generic;
using System.Linq;
using ViennaNET.Orm.Configuration;
using ViennaNET.Orm.Exceptions;
using Microsoft.Extensions.Configuration;

namespace ViennaNET.Orm.Factories
{
  /// <inheritdoc />
  public class SessionFactoryProvidersManager : ISessionFactoryProvidersManager
  {
    private readonly Lazy<IEnumerable<ISessionFactoryProvider>> _providers;

    /// <summary>
    /// Инициализирует экземпляр ссылками на <see cref="IConfiguration" /> и коллекцию <see cref="ISessionFactoryProviderGetter" />
    /// </summary>
    /// <param name="configuration">Ссылка на интерфейс, предоставляющий доступ к конфигурации</param>
    /// <param name="providerGetters">Ссылка на интерфейс, предоставляющий доступ списку фабрик провайдеров фабрик сессий</param>
    public SessionFactoryProvidersManager(IConfiguration configuration, IList<ISessionFactoryProviderGetter> providerGetters)
    {
      _providers =
        new Lazy<IEnumerable<ISessionFactoryProvider>>(() => GetSessionFactoryProvidersInternal(configuration, providerGetters));
    }

    private static IEnumerable<ISessionFactoryProvider> GetSessionFactoryProvidersInternal(
      IConfiguration configuration, IList<ISessionFactoryProviderGetter> providerGetters)
    {
      var result = new List<ISessionFactoryProvider>();
      var connectionInfos = configuration.GetSection("db")
                                         .Get<ConnectionInfo[]>();
      if (connectionInfos == null)
      {
        throw new SessionFactoryProviderException("There is no db section in the JSON configuration. Make sure you config is valid");
      }
      foreach (var connectionInfo in connectionInfos)
      {
        var providerGetter = providerGetters.SingleOrDefault(pr => pr.Type == connectionInfo.DbServerType);
        if (providerGetter == null)
        {
          throw new SessionFactoryProviderException($"Session factory provider getter with nick = {connectionInfo.Nick} is not found");
        }
        result.Add(providerGetter.GetSessionFactoryProvider(connectionInfo));
      }
      return result;
    }

    /// <inheritdoc />
    public ISessionFactoryProvider GetSessionFactoryProvider(string nick)
    {
      var provider = _providers.Value.SingleOrDefault(pr => pr.Nick == nick);
      if (provider == null)
      {
        throw new SessionFactoryProviderException($"Session factory provider with name={nick} is not found");
      }
      return provider;
    }

    /// <inheritdoc />
    public IEnumerable<ISessionFactoryProvider> GetSessionFactoryProviders()
    {
      return _providers.Value;
    }
  }
}
