using System.Linq;
using ViennaNET.Messaging.Resources.Impl;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Tools
{
  /// <summary>
  ///   Сериализатор/десериализатор для XML-сообщений
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class XmlResourcesMessageSerializer<T> : XmlMessageSerializerBase<T>
  {
    /// <summary>
    ///   Инициализирует экземпляр ссылкой на <see cref="IResourcesProvider" />
    /// </summary>
    /// <param name="resourcesProvider"></param>
    public XmlResourcesMessageSerializer(IResourcesProvider resourcesProvider)
    {
      xsd = resourcesProvider.ThrowIfNull(nameof(resourcesProvider))
                             .GetResourceStream<T>()
                             .ToList();
    }
  }
}