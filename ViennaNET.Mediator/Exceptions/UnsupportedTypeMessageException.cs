using System;
using System.Runtime.Serialization;

namespace ViennaNET.Mediator.Exceptions
{
  /// <summary>
  /// Исключение в случае, если переданный тип не соответствует ожидаемому
  /// </summary>
  [Serializable]
  public class UnsupportedTypeMessageException : Exception
  {
    /// <inheritdoc />
    public UnsupportedTypeMessageException(string message) 
      : base(message)
    {
    }

    protected UnsupportedTypeMessageException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
