using System;
using System.Runtime.Serialization;

namespace ViennaNET.Validation.Exceptions
{
  [Serializable]
  public class MessageValidationException : Exception
  {
    public MessageValidationException() { }

    public MessageValidationException(string message) : base(message) { }

    public MessageValidationException(string message, Exception innerException) : base(message, innerException) { }

    protected MessageValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
