using System;
using System.Threading;

namespace ViennaNET.Messaging.Extensions
{
  /// <summary>
  ///   Расширение для работы с тайм-аутами
  /// </summary>
  public static class TimeoutHelper
  {
    /// <summary>
    ///   Константа означающее отсутствие ожидания
    /// </summary>
    public static readonly long NoWaitTimeout = -1L;

    /// <summary>
    ///   Константа означающая бесконечное ожидание
    /// </summary>
    public static readonly long InfiniteWaitTimeout = 0L;

    /// <summary>
    ///   Получить тайм-аут в миллисекундах
    /// </summary>
    /// <param name="timeout">там-аут как TimeSpan</param>
    /// <returns>тайм-аут в миллисекундах</returns>
    public static long GetTimeout(TimeSpan? timeout)
    {
      if (timeout == Timeout.InfiniteTimeSpan || timeout == TimeSpan.MaxValue)
      {
        return InfiniteWaitTimeout;
      }

      if (timeout == TimeSpan.MinValue || timeout == null)
      {
        return NoWaitTimeout;
      }

      var waitTimeout = (long)timeout.Value.TotalMilliseconds;

      return waitTimeout <= 0L ? NoWaitTimeout : waitTimeout;
    }
  }
}