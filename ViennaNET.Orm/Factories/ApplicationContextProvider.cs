using System;
using System.Collections.Generic;
using System.Reflection;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm.Factories
{
  /// <inheritdoc />
  public class ApplicationContextProvider : IApplicationContextProvider
  {
    private const string DefaultNick = "default";

    private readonly Dictionary<Type, string> _commands = new Dictionary<Type, string>();
    private readonly Dictionary<Type, string> _entities = new Dictionary<Type, string>();
    private readonly Dictionary<string, string> _namedQueries = new Dictionary<string, string>();

    /// <summary>
    /// Инициализирует экземпляр ссылками на коллекцию <see cref="IBoundedContext" /> и <see cref="ISessionFactoryProvidersManager" />.
    /// Последний позволяет получить профайдер фабрик сессий и зарегистрировать в нем сущности для получения фабрики сессий
    /// </summary>
    /// <param name="boundedContexts">Ссылка на перечисление ограниченных контекстов</param>
    /// <param name="providersManager">Ссылка на интерфейс, представляющий менеджер провайдеров фабрик сессий</param>
    public ApplicationContextProvider(IEnumerable<IBoundedContext> boundedContexts, ISessionFactoryProvidersManager providersManager)
    {
      RegisterBoundedContexts(boundedContexts, providersManager);
    }

    /// <inheritdoc />
    public string GetNickForNamedQuery(string namedQuery)
    {
      if (_namedQueries.TryGetValue(namedQuery, out var nick))
      {
        return nick;
      }

      throw new EntityRepositoryException($"Named query {namedQuery} is not registered in factory");
    }

    /// <inheritdoc />
    public string GetNick(Type type)
    {
      if (_entities.TryGetValue(type, out var nick))
      {
        return nick;
      }

      throw new EntityRepositoryException($"Entity {type.Name} is not registered in factory");
    }

    /// <inheritdoc />
    public string GetNickForCommand(Type type)
    {
      if (_commands.TryGetValue(type, out var nick))
      {
        return nick;
      }

      throw new EntityRepositoryException($"Command {type.Name} is not registered in factory");
    }

    private void RegisterBoundedContexts(IEnumerable<IBoundedContext> boundedContexts, ISessionFactoryProvidersManager providersManager)
    {
      foreach (var boundedContext in boundedContexts)
      {
        foreach (var (type, nick, assembly) in boundedContext.Entities)
        {
          RegisterEntity(providersManager, type, nick, assembly);
        }

        if (boundedContext is IApplicationContext applicationContext)
        {
          foreach (var (type, nick, assembly) in applicationContext.IntegrationEvents)
          {
            RegisterEntity(providersManager, type, nick, assembly);
          }

          foreach (var (type, nick) in applicationContext.Commands)
          {
            RegisterCommand(type, nick);
          }

          foreach (var (queryName, nick) in applicationContext.NamedQueries)
          {
            RegisterNamedQuery(queryName, nick);
          }

          foreach (var (type, nick) in applicationContext.CustomQueries)
          {
            RegisterCustomQuery(type, nick);
          }
        }
      }
    }

    private void RegisterNamedQuery(string queryName, string nick)
    {
      if (_namedQueries.ContainsKey(queryName))
      {
        throw new EntityRepositoryException($"Named query {queryName} is already registered in factory");
      }

      _namedQueries.Add(queryName, nick ?? DefaultNick);
    }

    private void RegisterCommand(Type type, string nick)
    {
      if (_commands.ContainsKey(type))
      {
        throw new EntityRepositoryException($"Command {type.Name} is already registered in factory");
      }

      _commands.Add(type, nick ?? DefaultNick);
    }

    private void RegisterCustomQuery(Type type, string nick)
    {
      if (_entities.ContainsKey(type))
      {
        throw new EntityRepositoryException($"Value object {type.Name} is already registered in factory");
      }

      _entities.Add(type, nick ?? DefaultNick);
    }

    private void RegisterEntity(ISessionFactoryProvidersManager providersManager, Type type, string nick, Assembly assembly)
    {
      if (_entities.ContainsKey(type))
      {
        throw new EntityRepositoryException($"Entity {type.Name} is already registered in factory");
      }

      if (nick == null)
      {
        providersManager.GetSessionFactoryProvider(DefaultNick)
                        .AddClass(type, assembly);
        _entities.Add(type, DefaultNick);
      }
      else
      {
        providersManager.GetSessionFactoryProvider(nick)
                        .AddClass(type, assembly);
        _entities.Add(type, nick);
      }
    }
  }
}
