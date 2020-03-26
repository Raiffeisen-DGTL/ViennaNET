using System;
using System.Net.Sockets;
using ViennaNET.Logging;
using Polly;

namespace ViennaNET.ArcSight
{
  /// <inheritdoc />
  public class ErrorHandlingPoliciesFactory : IErrorHandlingPoliciesFactory
  {
    private const int retryInterval = 250;
    private const int retryCount = 3;

    /// <inheritdoc />
    public ISyncPolicy CreateStdCommunicationPolicy()
    {
      var countOfRetries = 0;
      return Policy.Handle<TimeoutException>()
                   .Or<SocketException>()
                   .Or<Exception>(InnerExceptionIsApplicable)
                   .WaitAndRetry(retryCount, retrySleep => TimeSpan.FromMilliseconds(retryInterval), (ex, time) =>
                   {
                     Logger.LogErrorFormat(ex, "Communication or Timeout exception. Retry: {0}", ++countOfRetries);
                   });
    }

    private static bool InnerExceptionIsApplicable(Exception ex)
    {
      return IsApplicable<TimeoutException>(ex) || IsApplicable<SocketException>(ex);
    }

    private static bool IsApplicable<T>(Exception ex) where T : Exception
    {
      while (ex != null)
      {
        if (ex is T)
        {
          return true;
        }

        ex = ex.InnerException;
      }

      return false;
    }
  }
}
