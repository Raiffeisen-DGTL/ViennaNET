using System.IO;
using Avro.IO;
using Avro.Specific;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.KafkaQueue
{
  /// <summary>
  ///   Сериализатор/десериализатор для Avro-сообщений
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class AvroMessageSerializer<T> : IMessageSerializer<T>, IMessageDeserializer<T> where T : ISpecificRecord, new()
  {
    /// <inheritdoc />
    public T Deserialize(BaseMessage message)
    {
      using (var ms = new MemoryStream(((BytesMessage)message).Body))
      {
        var dec = new BinaryDecoder(ms);
        var regenObj = new T();

        var reader = new SpecificDefaultReader(regenObj.Schema, regenObj.Schema);
        reader.Read(regenObj, dec);
        return regenObj;
      }
    }

    /// <inheritdoc />
    public BaseMessage Serialize(T message)
    {
      var result = new BytesMessage();
      using (var ms = new MemoryStream())
      {
        var enc = new BinaryEncoder(ms);
        var writer = new SpecificDefaultWriter(message.Schema); // Schema comes from pre-compiled, code-gen phase
        writer.Write(message, enc);
        result.Body = ms.ToArray();
      }

      return result;
    }
  }
}