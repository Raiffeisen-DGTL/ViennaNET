using System;

namespace ViennaNET.Redis.Exceptions
{
  /// <summary>
  /// Исключение, возникающее при наличии ошибки в конфигурации
  /// </summary>
  public class RedisConfigurationException : Exception
  {
    public RedisConfigurationException(string message, params object[] args) : base(string.Format(message, args))
    {
    }
  }
}
