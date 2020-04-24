using System;
using System.Collections.Generic;

namespace ViennaNET.Messaging.Resources
{
  /// <summary>
  /// Интерфейс базового хранилища ресурсов для сериализации сообщений
  /// </summary>
  public interface IResourcesStorage
  {
    /// <summary>
    /// Список соответствий типов сообщений и строк ресурсов
    /// </summary>
    IDictionary<Type, IEnumerable<string>> EmbeddedResources { get; }
  }
}