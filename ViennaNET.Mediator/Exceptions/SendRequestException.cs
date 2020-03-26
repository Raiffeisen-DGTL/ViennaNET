using System;
using System.Runtime.Serialization;

namespace ViennaNET.Mediator.Exceptions
{
  /// <summary>
  /// Исключение в процессе обработки запроса
  /// </summary>
  [Serializable]
  public class SendRequestException : Exception
  {
    /// <inheritdoc />
    public SendRequestException(string message) 
      : base(message)
    {
    }

    protected SendRequestException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
