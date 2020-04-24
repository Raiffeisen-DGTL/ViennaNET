using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ViennaNET.Utils;
using NHibernate;

namespace ViennaNET.Orm.Factories
{
  /// <inheritdoc />
  public class SessionFactoryManager : ISessionFactoryManager
  {
    private readonly OrderedDictionary _factories = new OrderedDictionary();
    private readonly object _lockObject = new object();
    private readonly ISessionFactoryProvidersManager _providersManager;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="ISessionFactoryProvidersManager" />
    /// </summary>
    /// <param name="providersManager">Ссылка на интерфейс, представляющий менеджер провайдеров фабрик сессий</param>
    public SessionFactoryManager(ISessionFactoryProvidersManager providersManager)
    {
      _providersManager = providersManager.ThrowIfNull(nameof(providersManager));
    }

    /// <inheritdoc />
    public ISessionFactory GetSessionFactory(string nick)
    {
      if (!_factories.Contains(nick))
      {
        lock (_lockObject)
        {
          if (!_factories.Contains(nick))
          {
            var factory = _providersManager.GetSessionFactoryProvider(nick)
                                           .GetSessionFactory();
            _factories.Add(nick, factory);
          }
        }
      }
      return (ISessionFactory)_factories[nick];
    }

    /// <inheritdoc />
    public IEnumerable<ISessionFactory> GetSessionFactories()
    {
      return _factories.Values.OfType<ISessionFactory>();
    }
  }
}
