using System;
using System.Runtime.Serialization;

namespace ViennaNET.Messaging.Exceptions
{
  /// <summary>
  /// Возникает при конфигурационных ошибках
  /// </summary>
  [Serializable]
  public class MessagingConfigurationException : MessagingException
  {
    /// <inheritdoc />
    public MessagingConfigurationException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public MessagingConfigurationException(Exception innerException, string message) :
      base(innerException, message)
    {
    }

    /// <inheritdoc />
    public MessagingConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}