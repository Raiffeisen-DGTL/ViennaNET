using System;

namespace ViennaNET.Messaging.Tools
{
  internal static class InternalTools
  {
    public static int CalculateTimeout(int errorCount, int errorThresholdCount, int reconnectTimeout, int maxReconnectTimeout)
    {
      // Examples of result:
      // errorCount = 0 , errorThresholdCount = 15, reconnectTimeout = 600, maxReconnectTimeout = 60000
      // Result: 600
      // errorCount = 1 , errorThresholdCount = 15, reconnectTimeout = 600, maxReconnectTimeout = 60000
      // Result: 600
      // errorCount = 15, errorThresholdCount = 15, reconnectTimeout = 600, maxReconnectTimeout = 60000
      // Result: 600
      // errorCount = 16, errorThresholdCount = 15, reconnectTimeout = 600, maxReconnectTimeout = 60000
      // Result: 1200
      // errorCount = 17, errorThresholdCount = 15, reconnectTimeout = 600, maxReconnectTimeout = 60000
      // Result: 2400
      // errorCount = 21, errorThresholdCount = 15, reconnectTimeout = 600, maxReconnectTimeout = 60000
      // Result: 38400
      // errorCount = 22, errorThresholdCount = 15, reconnectTimeout = 600, maxReconnectTimeout = 60000
      // Result: 60000
      // errorCount = 23, errorThresholdCount = 15, reconnectTimeout = 600, maxReconnectTimeout = 60000
      // Result: 60000

      if (errorCount > errorThresholdCount)
      {
        return Math.Min(reconnectTimeout * (int)Math.Pow(2, errorCount - errorThresholdCount > 20
                          ? 20
                          : errorCount - errorThresholdCount), maxReconnectTimeout);
      }

      return reconnectTimeout;
    }
  }
}
