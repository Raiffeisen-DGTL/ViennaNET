using System;
using System.Runtime.Serialization;

namespace ViennaNET.Messaging.Exceptions
{
  /// <inheritdoc />
  [Serializable]
  public class MessageProcessorAlreadyRegisterException : MessagingException
  {
    /// <inheritdoc />
    public MessageProcessorAlreadyRegisterException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public MessageProcessorAlreadyRegisterException(Exception innerException, string message) : base(innerException, message)
    {
    }

    /// <inheritdoc />
    protected MessageProcessorAlreadyRegisterException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
