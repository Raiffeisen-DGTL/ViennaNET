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
    private const int MaxLogLength = 100000;
    
    /// <summary>
    ///   Данные сообщения
    /// </summary>
    [NotNull]
    public string Body { get; set; }

    /// <summary>
    ///   Mime тип содержимого сообщения
    /// </summary>
    public string ContentType { get; set; } 

    /// <inheritdoc />
    public override string LogBody()
    {
      return Body?.Substring(0, Math.Min(Body.Length, MaxLogLength));
    }

    /// <inheritdoc cref="BaseMessage" />
    public override bool IsEmpty()
    {
      return string.IsNullOrEmpty(Body);
    }
  }
}