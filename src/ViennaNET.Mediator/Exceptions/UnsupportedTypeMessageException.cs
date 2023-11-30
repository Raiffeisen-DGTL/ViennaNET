using System;
using System.Runtime.Serialization;

namespace ViennaNET.Mediator.Exceptions
{
  /// <summary>
  ///   Исключение в случае, если переданный тип не соответствует ожидаемому
  /// </summary>
  [Serializable]
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
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