using System;
using System.Collections.Generic;
using System.Reflection;

namespace ViennaNET.Orm.Application
{
  /// <summary>
  /// Предназначен для регистрации команд и именовынных запросов
  /// Необходим для регистрации аппликационной части
  /// ограниченного контекста приложения.
  /// </summary>
  public interface IApplicationContext
  {
    /// <summary>
    /// Коллекция команд
    /// </summary>
    IReadOnlyCollection<(Type, string)> Commands { get; }

    /// <summary>
    /// Коллекция именованных запросов
    /// </summary>
    IReadOnlyCollection<(string, string)> NamedQueries { get; }

    /// <summary>
    /// Коллекция настраиваемых запросов
    /// </summary>
    IReadOnlyCollection<(Type, string)> CustomQueries { get; }

    /// <summary>
    /// Коллекция событий
    /// </summary>
    IReadOnlyCollection<(Type, string, Assembly)> IntegrationEvents { get; }
  }
}
