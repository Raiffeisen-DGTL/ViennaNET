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
    public override string LogBody()
    {
      return Body;
    }

    /// <inheritdoc cref="BaseMessage" />
    public override bool IsEmpty()
    {
      return string.IsNullOrEmpty(Body);
    }
  }
}