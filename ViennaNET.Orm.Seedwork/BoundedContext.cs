using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ViennaNET.Orm.Seedwork
{
  /// <inheritdoc />
  public abstract class BoundedContext : IBoundedContext
  {
    private readonly List<(Type, string, Assembly)> _entities;
    
    protected BoundedContext()
    {
      _entities = new List<(Type, string, Assembly)>();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<(Type, string, Assembly)> Entities => _entities.AsReadOnly();

    /// <inheritdoc />
    /// <exception cref="EntityRegistrationException">
    ///   Исключение, возникающее в случае если регистрируемая сущность не
    ///   реализует необходимый интерфейс
    /// </exception>
    public IBoundedContext AddEntity<T>(string nick = null, Assembly assembly = null) where T : class
    {
      var entityType = typeof(T);

      var isIEntityImplemented = entityType.GetInterfaces()
                                           .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityKey<>));
      if (!isIEntityImplemented)
      {
        throw new
          EntityRegistrationException($"All entities should implement IEntityKey interface. The entity of type {entityType} does not implement it.");
      }

      _entities.Add((typeof(T), nick, assembly));
      return this;
    }
  }
}
