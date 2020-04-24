using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm
{
  /// <summary>
  /// Позволяет регистрировать элементы ограниченного контекста приложения.
  /// Для использование необходимо создать единственного наследника данного класса
  /// внутри пользовательского приложения и зарегистрировать его в DI 
  /// </summary>
  public abstract class ApplicationContext : BoundedContext, IApplicationContext
  {
    private readonly List<(string, string)> _namedQueries;
    private readonly List<(Type, string)> _commands;
    private readonly List<(Type, string)> _customQueries;
    private readonly List<(Type, string, Assembly)> _integrationEvents;

    protected ApplicationContext()
    {
      _namedQueries = new List<(string, string)>();
      _commands = new List<(Type, string)>();
      _customQueries = new List<(Type, string)>();
      _integrationEvents = new List<(Type, string, Assembly)>();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<(string, string)> NamedQueries => _namedQueries.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<(Type, string)> Commands => _commands.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<(Type, string)> CustomQueries => _customQueries.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<(Type, string, Assembly)> IntegrationEvents => _integrationEvents.AsReadOnly();

    /// <inheritdoc />
    public IApplicationContext AddCommand<T>(string nick = null) where T : class
    {
      _commands.Add((typeof(T), nick));
      return this;
    }

    /// <inheritdoc />
    public IApplicationContext AddNamedQuery<T>(string queryName, string dbNick = null) where T : class
    {
      _namedQueries.Add((queryName, dbNick));
      return this;
    }

    /// <inheritdoc />
    public IApplicationContext AddCustomQuery<T>(string nick = null) where T : class
    {
      _customQueries.Add((typeof(T), nick));
      return this;
    }

    /// <inheritdoc />
    /// <exception cref="IntegrationEventMappingRegistrationException">
    ///   Исключение, возникающее в случае если регистрируемая сущность не
    ///   реализует необходимый интерфейс
    /// </exception>
    public IApplicationContext AddIntegrationEvent<T>(string nick = null, Assembly assembly = null) where T : class
    {
      var eventType = typeof(T);

      var isIEventImplemented = eventType.GetInterfaces()
                                         .Any(i => i == typeof(IIntegrationEvent));
      if (!isIEventImplemented)
      {
        throw new
          IntegrationEventMappingRegistrationException($"All integration events should implement IIntegrationEvent interface. The event of type {eventType} does not implement it.");
      }

      _integrationEvents.Add((typeof(T), nick, assembly));
      return this;
    }
  }
}
