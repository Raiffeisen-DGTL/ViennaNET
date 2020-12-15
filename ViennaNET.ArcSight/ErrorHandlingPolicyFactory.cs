using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;

namespace ViennaNET.ArcSight
{
  /// <inheritdoc />
  public class ErrorHandlingPoliciesFactory : IErrorHandlingPoliciesFactory
  {
    private readonly ILogger<ErrorHandlingPoliciesFactory> _logger;
    private const int retryInterval = 250;

    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="logger">A logger interface</param>
    public ErrorHandlingPoliciesFactory(ILogger<ErrorHandlingPoliciesFactory> logger)
    {
      _logger = logger;
    }

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
                     _logger.LogError(ex, $"Communication or Timeout exception. Retry: {++countOfRetries}");
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
