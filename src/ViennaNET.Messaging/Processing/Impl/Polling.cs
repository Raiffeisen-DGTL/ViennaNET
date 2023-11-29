using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Processing.Impl
{
  /// <summary>
  ///   Реализует алгоритм поллинга
  /// </summary>
  public sealed class Polling : IDisposable
  {
    private readonly ILogger _logger;
    private readonly Func<CancellationToken, Task<bool>> _processAction;

    private readonly object _stateLock = new();
    private readonly int _timeoutPollingQueue;

    private CancellationTokenSource _cancellationTokenSource;
    private bool _isDisposed;
    private Task _task;

    /// <summary>
    ///   Инициализирует экземпляр значением интервала опроса и идентификатором потока
    /// </summary>
    /// <param name="timeoutPollingQueue">Интервал опроса, мс</param>
    /// <param name="processAction">
    ///   <see cref="Func{TResult}" />, осуществляющий непосредственную работу с очередью.
    ///   Возвращаемое значение определяет, было ли прочитано сообщение из очереди.
    /// </param>
    /// <param name="logger">Интерфейс логгирования</param>
    /// <remarks>
    ///   В зависимости от возвращаемого значения processAction процесс ждёт заданные интервал (если сообщение не было
    ///   прчитано) или сразу пытается читать следующее (если было).
    /// </remarks>
    public Polling(int timeoutPollingQueue, Func<CancellationToken, Task<bool>> processAction, ILogger logger)
    {
      _timeoutPollingQueue = timeoutPollingQueue;
      _processAction = processAction.ThrowIfNull(nameof(processAction));
      _logger = logger.ThrowIfNull(nameof(logger));
    }

    /// <summary>
    ///   Статус запуска
    /// </summary>
    public bool IsStarted
    {
      get
      {
        CheckDisposed();
        return _task != null;
      }
    }

    /// <inheritdoc />
    public void Dispose()
    {
      Dispose(true);
      _isDisposed = true;
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Запуск процесса прослушивания очередей
    /// </summary>
    public void StartPolling()
    {
      CheckDisposed();

      if (IsStarted)
      {
        return;
      }

      lock (_stateLock)
      {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        _task = Task.Run(async () => await ProcessAsync(cancellationToken).ConfigureAwait(false), cancellationToken);
      }
    }

    private async Task ProcessAsync(CancellationToken ct)
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
        catch (TaskCanceledException)
        {
          // Everything's under control, just end the task
        }
        catch (Exception e)
        {
          _logger.LogError(e, "Error while executing polling");
        }
      }
    }

    private void CheckDisposed()
    {
      if (_isDisposed)
      {
        throw new ObjectDisposedException(nameof(Polling));
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
    ~Polling()
    {
      Dispose(false);
    }
  }
}