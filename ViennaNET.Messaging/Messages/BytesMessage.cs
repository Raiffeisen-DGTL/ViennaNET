using System;
using System.Text;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Messages
{
  /// <summary>
  ///   Сообщение
  /// </summary>
  [Serializable]
  public class BytesMessage : BaseMessage
  {
    /// <summary>
    ///   Данные сообщения
    /// </summary>
    [NotNull]
    public byte[] Body { get; set; }

    /// <inheritdoc />
    public override string GetBodyAsString()
    {
      return Body == null
        ? string.Empty
        : Encoding.UTF8.GetString(Body);
    }
  }
}