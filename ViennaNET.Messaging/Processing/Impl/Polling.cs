using System;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Logging;

namespace ViennaNET.Messaging.Processing.Impl
{
  /// <inheritdoc />
  public class Polling : IPolling
  {
    private readonly int _timeoutPollingQueue;
    private readonly string _pollingId;

    private CancellationTokenSource _cancellationTokenSource;
    private Task _task;

    private readonly object _stateLock = new object();

    /// <summary>
    /// Инициализирует экземпляр значением интервала опроса и идентификатором потока
    /// </summary>
    /// <param name="timeoutPollingQueue">Интервал опроса</param>
    /// <param name="pollingId">Идентификатор потока для опроса</param>
    public Polling(int timeoutPollingQueue, string pollingId)
    {
      _timeoutPollingQueue = timeoutPollingQueue;
      _pollingId = pollingId;
    }

    /// <inheritdoc />
    public bool IsStarted { get; private set; }

    /// <inheritdoc />
    public void StartPolling(Func<CancellationToken, Task> processAction)
    {
      if (IsStarted)
      {
        return;
      }

      lock (_stateLock)
      {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        _task = Task.Run(async () =>
        {
          if (!string.IsNullOrWhiteSpace(_pollingId))
          {
            Logger.RequestId = _pollingId;
          }

          while (!cancellationToken.IsCancellationRequested)
          {
            try
            {
              await processAction(cancellationToken);
              await Task.Delay(_timeoutPollingQueue, cancellationToken);
            }
            catch (TaskCanceledException)
            {
              // Everything's under control
            }
            catch (Exception e)
            {
              Logger.LogError(e, "Error while executing polling");
            }
          }
        }, cancellationToken);

        IsStarted = _task.Status == TaskStatus.WaitingForActivation;
      }
    }

    /// <inheritdoc />
    public void StopPolling()
    {
      if (!IsStarted)
      {
        return;
      }

      lock (_stateLock)
      {
        IsStarted = false;

        _cancellationTokenSource.Cancel();

        _task.GetAwaiter().GetResult();
      }
    }
  }
}
