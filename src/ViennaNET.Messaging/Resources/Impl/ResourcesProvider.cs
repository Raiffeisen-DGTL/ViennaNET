using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Resources.Impl
{
  /// <summary>
  ///   Реализация провайдера путей ресурсов для сериализации
  /// </summary>
  public class ResourcesProvider : IResourcesProvider
  {
    private readonly IResourcesLoader _embeddedResourcesLoader;
    private readonly IDictionary<Type, IEnumerable<string>> _resources;

    /// <summary>
    ///   Инициализирует экземпляр класса ссылками на <see cref="IResourcesLoader" /> и на коллекцию
    ///   <see cref="IResourcesStorage" />
    /// </summary>
    /// <param name="resources"></param>
    /// <param name="embeddedResourcesLoader"></param>
    /// <exception cref="MessagingConfigurationException"></exception>
    public ResourcesProvider(IEnumerable<IResourcesStorage> resources, IResourcesLoader embeddedResourcesLoader)
    {
      _embeddedResourcesLoader = embeddedResourcesLoader.ThrowIfNull(nameof(resources));

      _resources = new Dictionary<Type, IEnumerable<string>>();
      foreach (var resource in resources.SelectMany(x => x.EmbeddedResources))
      {
        if (_resources.ContainsKey(resource.Key))
        {
          throw new
            MessagingConfigurationException($"An embedded resource path for type {resource.Key} has already been registered in ResourceProvider for the message serialization");
        }

        _resources.Add(resource);
      }
    }

    /// <inheritdoc />
    public IEnumerable<Stream> GetResourceStream<T>()
    {
      if (!_resources.TryGetValue(typeof(T), out var paths))
      {
        throw new
          MessagingConfigurationException($"There is no registered embedded resource paths for the message serialization for the type {typeof(T)}");
      }

      try
      {
        return paths.Select(x => _embeddedResourcesLoader.GetResourceStream<T>(x));
      }
      catch (Exception e)
      {
        throw new MessagingConfigurationException(e,
                                                  $"Cannot load embedded resource for the type {typeof(T)} from the path {paths}. Probably you forgot to make your resource as Embedded Resource?");
      }
    }
  }
}