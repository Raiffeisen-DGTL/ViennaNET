using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tools
{
  /// <summary>
  ///   Сериализатор/десериализатор для XML-сообщений
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class XmlMessageSerializerBase<T> : IMessageSerializer<T>, IMessageDeserializer<T>
  {
    /// <summary>
    ///   Набор подгруженных xsd-схем для валидации сообщения
    /// </summary>
    protected IList<Stream> xsd = new List<Stream>();

    /// <inheritdoc />
    public T Deserialize(BaseMessage message)
    {
      ValidateXml(message.GetBodyAsString());
      var serializer = new XmlSerializer(typeof(T));
      var reader = new StringReader(message.GetBodyAsString());
      return (T)serializer.Deserialize(reader);
    }

    /// <inheritdoc />
    public BaseMessage Serialize(T message)
    {
      var result = new TextMessage();
      var writer = new StringWriterUtf8();
      new XmlSerializer(typeof(T)).Serialize(writer, message);
      var xml = writer.ToString();
      ValidateXml(xml);
      result.Body = xml;
      return result;
    }

    protected void ValidateXml(string xml)
    {
      if (!xsd.Any())
      {
        return;
      }

      var doc = new XmlDocument();
      foreach (var stream in xsd)
      {
        doc.Schemas.Add(XmlSchema.Read(stream, null));
      }

      doc.LoadXml(xml);
      doc.Validate((sender, args) => throw args.Exception);
    }

    private class StringWriterUtf8 : StringWriter
    {
      public override Encoding Encoding => Encoding.UTF8;
    }
  }
}