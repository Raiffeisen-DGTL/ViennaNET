using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ViennaNET.Orm.Seedwork
{
  /// <inheritdoc />
  public abstract class BoundedContext : IBoundedContext
  {
    private readonly List<(Type, string, Assembly)> _entities = new List<(Type, string, Assembly)>();

    /// <inheritdoc />
    public IReadOnlyCollection<(Type, string, Assembly)> Entities => _entities.AsReadOnly();

    /// <summary>
    /// Позволяет добавить новую сущность в контекст
    /// </summary>
    /// <param name="dbNick">Имя подключения к БД в файле конфигурации</param>
    /// <param name="assembly">Сборка в которой объявлена сущность</param>
    /// <typeparam name="T">Класс сущности. Должен реализовывать <see cref="IEntityKey{TKey}"/></typeparam>
    /// <exception cref="EntityRegistrationException">
    ///   Исключение, возникающее в случае если регистрируемая сущность не
    ///   реализует необходимый интерфейс
    /// </exception>
    protected IBoundedContext AddEntity<T>(string dbNick = null, Assembly assembly = null) where T : class, IEntityKey
    {
      var entityType = typeof(T);

      var isIEntityImplemented = entityType
        .GetInterfaces()
        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityKey<>));
      if (!isIEntityImplemented)
      {
        throw new
          EntityRegistrationException($"All entities should implement IEntityKey interface. The entity of type {entityType} does not implement it.");
      }

      _entities.Add((entityType, dbNick, assembly));
      return this;
    }


    /// <summary>
    /// Добавляет все сущности в сборке в контекст.
    /// </summary>
    /// <param name="dbNick">Имя подключения к БД в файле конфигурации</param>
    /// /// <param name="assembly">Сборка в которой ищутся сущности</param>
    /// <returns>Себя</returns>
    /// <remarks>Поиск сущностей ведётся по реализации классом интерфейса <see cref="IEntityKey{TKey}"/></remarks>
    protected IBoundedContext AddAllEntities(string dbNick = null, Assembly assembly = null)
    {
      var myAssembly = assembly ?? Assembly.GetCallingAssembly();
      var entities = myAssembly
        .GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityKey<>)));

      foreach (var entity in entities)
      {
        var isBaseClass = entities.Any(e => e != entity && entity.IsAssignableFrom(e));
        if (!isBaseClass)
        {
          _entities.Add((entity, dbNick, assembly));
        }
      }

      return this;
    }
  }
}