using System.Collections.Generic;
using System.IO;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Tools
{
  /// <summary>
  ///   Сериализатор/десериализатор для XML-сообщений
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class XmlMessageSerializer<T> : XmlMessageSerializerBase<T>
  {
    /// <summary>
    ///   Устанавливает XSD для сообщений
    /// </summary>
    /// <param name="stream">
    ///   <see cref="Stream" />
    /// </param>
    public void SetStream(Stream stream)
    {
      xsd = new List<Stream> { stream.ThrowIfNull(nameof(stream)) };
    }

    /// <summary>
    ///   Добавляет XSD для сообщений
    /// </summary>
    /// <param name="stream">
    ///   <see cref="Stream" />
    /// </param>
    public void AddStream(Stream stream)
    {
      xsd.Add(stream.ThrowIfNull(nameof(stream)));
    }
  }
}