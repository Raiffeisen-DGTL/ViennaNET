using System;
using System.Collections.Generic;

namespace ViennaNET.Messaging.Resources.Impl
{
  /// <summary>
  ///   Базовый класс хранилища ресурсов для сериализации сообщений
  /// </summary>
  public abstract class ResourcesStorage : IResourcesStorage
  {
    /// <inheritdoc />
    public IDictionary<Type, IEnumerable<string>> EmbeddedResources { get; } = new Dictionary<Type, IEnumerable<string>>();

    /// <summary>
    ///   Помещает соответствие типа сообщения и строки пути в хранилище
    /// </summary>
    /// <param name="type">Тип сообщения</param>
    /// <param name="path">Строка пути</param>
    protected void SetEmbeddedResourceDefinition(Type type, IEnumerable<string> path)
    {
      EmbeddedResources[type] = path;
    }

    protected void SetEmbeddedResourceDefinition(Type type, string path)
    {
      EmbeddedResources[type] = new[] { path };
    }
  }
}