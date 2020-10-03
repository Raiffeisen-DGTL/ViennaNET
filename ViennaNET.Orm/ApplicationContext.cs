using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Repositories;
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
    private readonly List<(string, string)> _namedQueries = new List<(string, string)>();
    private readonly List<(Type, string)> _commands = new List<(Type, string)>();
    private readonly List<(Type, string)> _customQueries = new List<(Type, string)>();
    private readonly List<(Type, string, Assembly)> _integrationEvents = new List<(Type, string, Assembly)>();

    /// <inheritdoc />
    public IReadOnlyCollection<(string, string)> NamedQueries => _namedQueries.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<(Type, string)> Commands => _commands.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<(Type, string)> CustomQueries => _customQueries.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<(Type, string, Assembly)> IntegrationEvents => _integrationEvents.AsReadOnly();

    /// <summary>
    /// Позволяет добавить новую команду в контекст
    /// </summary>
    /// <typeparam name="T">Тип регистрируемой команды</typeparam>
    /// <param name="dbNick">Имя подключения к БД в файле конфигурации</param>
    protected IApplicationContext AddCommand<T>(string dbNick = null) where T : class, ICommand
    {
      _commands.Add((typeof(T), dbNick));
      return this;
    }

    /// <summary>
    /// Регистрирует все команды в сборке
    /// </summary>
    /// <param name="dbNick">Имя подключения к БД в файле конфигурации</param>
    /// <param name="assembly">Сборка в которой ведётся поиск команд. По умолчанию - <see cref="Assembly.GetCallingAssembly"/>.</param>
    /// <returns>Себя</returns>
    /// <remarks>Поиск команд ведётся по реализации от <see cref="ICommand"/></remarks>
    protected IApplicationContext AddAllCommands(string dbNick = null, Assembly assembly = null)
    {
      var myAssembly = assembly ?? Assembly.GetCallingAssembly();
      var commands = myAssembly
        .GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(ICommand)))
        .Select(t => (t, dbNick));

      _commands.AddRange(commands);

      return this;
    }

    /// <summary>
    /// Позволяет добавить новый именованный запрос в контекст
    /// </summary>
    /// <param name="queryName">Имя регистрируемого запроса</param>
    /// <param name="dbNick">Имя подключения к БД в файле конфигурации</param>
    protected IApplicationContext AddNamedQuery(string queryName, string dbNick = null)
    {
      _namedQueries.Add((queryName, dbNick));
      return this;
    }

    /// <summary>
    /// Позволяет добавить новый настраиваемый запрос в контекст
    /// </summary>
    /// <typeparam name="T">Тип сущности, возвращаемой запросом</typeparam>
    /// <param name="dbNick">Имя подключения к БД в файле конфигурации</param>
    protected IApplicationContext AddCustomQuery<T>(string dbNick = null) where T : class
    {
      _customQueries.Add((typeof(T), dbNick));
      return this;
    }

    /// <summary>
    /// Позволяет добавить новое событие для публикации в контекст
    /// </summary>
    /// <typeparam name="T">Тип регистрируемого события</typeparam>
    /// <param name="dbNick">Имя подключения к БД в файле конфигурации</param>
    /// <param name="assembly">Сборка в которой находится класс события</param>
    /// <returns></returns>
    protected IApplicationContext AddIntegrationEvent<T>(string dbNick = null, Assembly assembly = null) where T : class, IIntegrationEvent
    {
      _integrationEvents.Add((typeof(T), dbNick, assembly));
      return this;
    }


    /// <summary>
    /// Регистрирует все события в сборке
    /// </summary>
    /// <param name="dbNick">Имя подключения к БД в файле конфигурации</param>
    /// <param name="assembly">Сборка в которой ведётся поиск событий. По умолчанию - <see cref="Assembly.GetCallingAssembly"/>.</param>
    /// <returns>Себя</returns>
    /// <remarks>Поиск событий ведётся по реализации классом интерфейса <see cref="IIntegrationEvent"/></remarks>
    protected IApplicationContext AddAllIntegrationEvents(string dbNick = null, Assembly assembly = null)
    {
      var myAssembly = assembly ?? Assembly.GetCallingAssembly();
      var events = myAssembly
        .GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IIntegrationEvent)))
        .Select(t => (t, dbNick, assembly));

      _integrationEvents.AddRange(events);

      return this;
    }
  }
}
