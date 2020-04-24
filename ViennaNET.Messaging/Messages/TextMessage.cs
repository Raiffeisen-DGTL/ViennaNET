using System;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Messages
{
  /// <summary>
  ///   Сообщение
  /// </summary>
  [Serializable]
  public class TextMessage : BaseMessage
  {
    /// <summary>
    ///   Данные сообщения
    /// </summary>
    [NotNull]
    public string Body { get; set; }

    /// <inheritdoc />
    public override string GetBodyAsString()
    {
      return Body;
    }
  }
}