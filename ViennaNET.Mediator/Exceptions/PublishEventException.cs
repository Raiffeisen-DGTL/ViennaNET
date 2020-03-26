using System;
using System.Runtime.Serialization;

namespace ViennaNET.Mediator.Exceptions
{
  /// <summary>
  /// Исключение в процессе публикации доменного события
  /// </summary>
  [Serializable]
  public class PublishEventException : Exception
  {
    /// <inheritdoc />
    public PublishEventException(string message) 
      : base(message)
    {
    }

    protected PublishEventException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
