using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tools
{
  /// <summary>
  /// Serializer/Deserializer for JSON messages
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class JsonMessageSerializer<T> : IMessageSerializer<T>, IMessageDeserializer<T>
  {
    private JsonSerializerOptions options =>
      new JsonSerializerOptions
      {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic), IgnoreNullValues = true
      };

    /// <inheritdoc />
    public T Deserialize(BaseMessage message)
    {
      var stringBody = ((TextMessage)message).Body;
      return JsonSerializer.Deserialize<T>(stringBody, options);
    }

    /// <inheritdoc />
    public BaseMessage Serialize(T message)
    {
      var result = new TextMessage();
      var json = JsonSerializer.Serialize(message, options);
      result.Body = json;

      return result;
    }
  }
}
