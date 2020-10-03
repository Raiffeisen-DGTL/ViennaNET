using System;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Logging;

namespace ViennaNET.Messaging.Processing.Impl
{
  /// <summary>
  /// Реализует алгоритм поллинга
  /// </summary>
  public sealed class Polling : IDisposable
  {
    private readonly int _timeoutPollingQueue;
    private readonly string _pollingId;
    private readonly Func<CancellationToken, Task<bool>> _processAction;

    private readonly object _stateLock = new object();

    private CancellationTokenSource _cancellationTokenSource;
    private Task _task;

    /// <summary>
    /// Инициализирует экземпляр значением интервала опроса и идентификатором потока
    /// </summary>
    /// <param name="timeoutPollingQueue">Интервал опроса, мс</param>
    /// <param name="processAction"><see cref="Func{TResult}"/>, осуществляющий непосредственную работу с очередью. Возвращаемое значение определяет, было ли прочитано сообщение из очереди.</param>
    /// <param name="pollingId">Идентификатор потока для опроса</param>
    /// <remarks>В зависимости от возвращаемого значения processAction процесс ждёт заданные интервал (если сообщение не было прчитано) или сразу пытается читать следующее (если было).</remarks>
    public Polling(int timeoutPollingQueue, Func<CancellationToken, Task<bool>> processAction, string pollingId = null)
    {
      _timeoutPollingQueue = timeoutPollingQueue;
      _pollingId = pollingId;
      _processAction = processAction ?? throw new ArgumentNullException(nameof(processAction));
    }

    /// <summary>
    /// Статус запуска
    /// </summary>
    public bool IsStarted => _task != null;

    /// <summary>
    /// Запуск процесса прослушивания очередей
    /// </summary>
    public void StartPolling()
    {
      if (IsStarted)
      {
        return;
      }

      lock (_stateLock)
      {
        if (!string.IsNullOrWhiteSpace(_pollingId))
        {
          Logger.RequestId = _pollingId;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        _task = Task.Run(async () => await ProcessAsync(cancellationToken).ConfigureAwait(false), cancellationToken);
      }
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
      try
      {
        while (!ct.IsCancellationRequested)
        {
          try
          {
            var messageReceived = await _processAction(ct).ConfigureAwait(false);
            if (!messageReceived)
            {
              await Task.Delay(_timeoutPollingQueue, ct).ConfigureAwait(false);
            }
          }
          catch (Exception e)
          {
            Logger.LogError(e, "Error while executing polling");
          }
        }
      }
      catch (TaskCanceledException)
      {
        // Everything's under control, just end task
      }
    }

    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        lock (_stateLock)
        {
          if (_cancellationTokenSource != null)
          {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
          }

          if (_task != null)
          {
            try
            {
              _task.GetAwaiter().GetResult();
            }
            catch (TaskCanceledException)
            {
              // It's OK
            }

            _task.Dispose();
            _task = null;
          }
        }
      }
    }

    /// <inheritdoc />
    public void Dispose()
    {
      Dispose(true);
    }

    /// <inheritdoc />
    ~Polling()
    {
      Dispose(false);
    }
  }
}
