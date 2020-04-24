using System;
using System.Collections.Generic;
using System.Reflection;

namespace ViennaNET.Orm.Seedwork
{
  /// <summary>
  /// Предназначен для регистрации сущностей и объектов-значений
  /// Необходим для регистрации доменной части
  /// ограниченного контекста приложения.
  /// </summary>
  public interface IBoundedContext
  {
    /// <summary>
    /// Коллекция сущностей
    /// </summary>
    IReadOnlyCollection<(Type, string, Assembly)> Entities { get; }

    /// <summary>
    /// Позволяет добавить новую сущность в контекст
    /// </summary>
    IBoundedContext AddEntity<T>(string nick = null, Assembly assembly = null) where T : class;
  }
}
