using System;
using System.Threading;

namespace ViennaNET.Messaging.MQSeriesQueue.Infrastructure
{
  internal static class TimeoutHelper
  {
    public const long NoWaitTimeout = -1L;
    public const long InfiniteWaitTimeout = 0L;

    public     static long GetTimeout(TimeSpan? timeout)
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
