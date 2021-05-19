using System;
using System.Runtime.Serialization;

namespace ViennaNET.Messaging.Exceptions
{
  /// <summary>
  ///   Исключение об отсутствии сообщения в очереди при чтении
  /// </summary>
  [Serializable]
  public sealed class MessageDidNotReceivedException : MessagingException
  {
    /// <inheritdoc />
    public MessageDidNotReceivedException(Exception innerException, string message) : base(innerException, message)
    {
    }

    /// <inheritdoc />
    public MessageDidNotReceivedException(string message) : base(message)
    {
    }

    private MessageDidNotReceivedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}