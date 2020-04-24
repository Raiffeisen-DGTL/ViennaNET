using System;
using System.Runtime.Serialization;

namespace ViennaNET.Messaging.Exceptions
{
  /// <summary>
  ///   Базовый класс исключения для модуля очередей
  /// </summary>
  [Serializable]
  public class MessagingException : Exception
  {
    /// <inheritdoc />
    public MessagingException(Exception innerException, string message) : base(message, innerException)
    {
    }

    /// <inheritdoc />
    public MessagingException(string message) : base(message)
    {
      
    }

    /// <inheritdoc />
    public MessagingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
