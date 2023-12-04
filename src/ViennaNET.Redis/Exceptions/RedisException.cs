using System;

namespace ViennaNET.Redis.Exceptions
{
  /// <summary>
  ///   Исключение, возникающее при ошибках во время выполнения операций с Redis
  /// </summary>
  public class RedisException : Exception
  {
    /// <inheritdoc />
    public RedisException()
    {
    }

    /// <inheritdoc />
    public RedisException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public RedisException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}