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
    /// Позволяет добавить новую команду в контекст
    /// </summary>
    IApplicationContext AddCommand<T>(string nick = null) where T : class;

    /// <summary>
    /// Позволяет добавить новый именованный запрос в контекст
    /// </summary>
    IApplicationContext AddNamedQuery<T>(string queryName, string dbNick = null) where T : class;

    /// <summary>
    /// Позволяет добавить новый настраиваемый запрос в контекст
    /// </summary>
    IApplicationContext AddCustomQuery<T>(string nick = null) where T : class;

    /// <summary>
    /// Коллекция событий
    /// </summary>
    IReadOnlyCollection<(Type, string, Assembly)> IntegrationEvents { get; }

    /// <summary>
    /// Позволяет добавить новое событие для публикации в контекст
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nick"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    IApplicationContext AddIntegrationEvent<T>(string nick = null, Assembly assembly = null) where T : class;
  }
}
