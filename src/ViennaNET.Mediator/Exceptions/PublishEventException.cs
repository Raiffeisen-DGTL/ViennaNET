using System;
using System.Runtime.Serialization;

namespace ViennaNET.Mediator.Exceptions
{
  /// <summary>
  ///   Исключение в процессе публикации доменного события
  /// </summary>
  [Serializable]
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
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