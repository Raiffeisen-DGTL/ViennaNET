using System;
using System.Runtime.Serialization;

namespace ViennaNET.Mediator.Exceptions
{
  /// <summary>
  ///   Исключение в процессе обработки запроса
  /// </summary>
  [Serializable]
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
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