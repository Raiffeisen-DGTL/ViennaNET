using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Diagnostic;
using ViennaNET.Logging;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Processing.Impl.Poll
{
  /// <inheritdoc/>
  public abstract class QueuePollingReactorBase : IQueueReactor
  {
    private const int ErrorThresholdCount = 15;
    private const int DefaultReconnectTimeout = 600;
    private const int MaxReconnectTimeout = 60000;

    private readonly IMessageAdapter _adapter;
    private readonly IEnumerable<IMessageProcessor> _messageProcessors;
    private readonly IEnumerable<IMessageProcessorAsync> _asyncMessageProcessors;
    private readonly IPolling _subscribePolling;

    private readonly IMessagingCallContextAccessor _messagingCallContextAccessor;
    private readonly IHealthCheckingService _healthCheckingService;
    private readonly bool _serviceHealthDependent;
    private bool _hasDiagnosticErrors;

    private bool _isDisposed;
    private int _errorCount;

    /// <summary>
    /// Инициализирует компонент ссылками на <see cref="IMessageAdapter"/>, <see cref="IHealthCheckingService"/>,
    /// коллекции <see cref="IMessageProcessor"/>, <see cref="IMessageProcessorAsync"/>
    /// </summary>
    /// <param name="messageAdapter">Адаптер для работы с очередью</param>
    /// <param name="messageProcessors">Процессоры для обработки сообщения</param>
    /// <param name="asyncMessageProcessors">Асинхронные процессоры для обработки сообщения</param>
    /// <param name="subscribeInterval">Интервал подписки</param>
    /// <param name="pollingId">Идентификатор потока для опроса</param>
    /// <param name="serviceHealthDependent">Признак использования диагностики</param>
    /// <param name="healthCheckingService">Ссылка на службу диагностики</param>
    /// <param name="messagingCallContextAccessor">Контейнер с параметрами вызова для сообщения</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected QueuePollingReactorBase(IMessageAdapter messageAdapter,
                                      IEnumerable<IMessageProcessor> messageProcessors,
                                      IEnumerable<IMessageProcessorAsync> asyncMessageProcessors,
                                      int subscribeInterval,
                                      string pollingId,
                                      bool? serviceHealthDependent,
                                      IHealthCheckingService healthCheckingService,
                                      IMessagingCallContextAccessor messagingCallContextAccessor)
    {
      _adapter = messageAdapter.ThrowIfNull(nameof(messageAdapter));
      _messageProcessors = messageProcessors.ThrowIfNull(nameof(messageProcessors));
      _asyncMessageProcessors = asyncMessageProcessors.ThrowIfNull(nameof(asyncMessageProcessors));
      if (subscribeInterval <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(subscribeInterval));
      }
      _healthCheckingService = healthCheckingService.ThrowIfNull(nameof(serviceHealthDependent));
      _hasDiagnosticErrors = false;
      _serviceHealthDependent = serviceHealthDependent ?? false;

      _messagingCallContextAccessor = messagingCallContextAccessor;

      _subscribePolling = new Polling(subscribeInterval, pollingId);
    }

    /// <inheritdoc/>
    public bool StartProcessing()
    {
      if (_subscribePolling.IsStarted)
      {
        return false;
      }

      if (_serviceHealthDependent)
      {
        _healthCheckingService.DiagnosticPassedEvent += OnDiagnosticPassed;
        _healthCheckingService.DiagnosticFailedEvent += OnDiagnosticFailed;
      }

      _subscribePolling.StartPolling(ListenMessagesAsync);
      return _subscribePolling.IsStarted;
    }

    private void OnDiagnosticFailed()
    {
      _hasDiagnosticErrors = true;
      Logger.LogDebug("QueuePollingReactor: Service diagnostic failed, stop listening");
    }

    private void OnDiagnosticPassed()
    {
      _hasDiagnosticErrors = false;
      Logger.LogDebug("QueuePollingReactor: Service diagnostic passed");
    }

    /// <inheritdoc/>
    public void Stop()
    {
      try
      {
        if (_serviceHealthDependent)
        {
          _healthCheckingService.DiagnosticFailedEvent -= OnDiagnosticFailed;
          _healthCheckingService.DiagnosticPassedEvent -= OnDiagnosticPassed;
        }

        _subscribePolling.StopPolling();
      }
      finally
      {
        _adapter.Disconnect();
      }
    }

    /// <inheritdoc />
    public event EventHandler<EventArgs> NeedReconnect = delegate { };

    /// <inheritdoc />
    public int ErrorCount => _errorCount;

    /// <summary>
    /// Отправляет сообщение в цепочку процессоров для последующей обработки в асинхронном режиме
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>Признак успешности обработки</returns>
    protected bool GetProcessedMessage(BaseMessage message)
    {
      return _messageProcessors.Any(processor => processor.Process(message));
    }

    /// <summary>
    /// Отправляет сообщение в цепочку процессоров для последующей обработки
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>Признак успешности обработки</returns>
    protected async Task<bool> GetProcessedMessageAsync(BaseMessage message)
    {
      var processed = false;
      foreach (var processor in _asyncMessageProcessors)
      {
        processed = await processor.ProcessAsync(message);
        if (processed)
        {
          break;
        }
      }

      return processed;
    }

    private void NotifyNeedReconnect()
    {
      try
      {
        NeedReconnect?.Invoke(this, EventArgs.Empty);
      }
      catch (Exception e)
      {
        Logger.LogError(e, "Error while execute reconnect event");
      }
    }

    private async Task ListenMessagesAsync(CancellationToken cancellationToken)
    {
      try
      {
        Logger.LogDebug("Listen messages ...");

        if (_serviceHealthDependent && _hasDiagnosticErrors)
        {
          Logger.LogDebug("Diagnostic not passed. Skip listening");
          return;
        }

        if (!_adapter.IsConnected)
        {
          _adapter.Connect();
        }

        if (_adapter.TryReceive(out var message))
        {
          SetCallContextFromMessage(message);
          await ProcessReceivedMessage(message);
        }

        Interlocked.Exchange(ref _errorCount, 0);
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, "Error during message processing");
        await ReconnectAfterErrorAsync(cancellationToken);
      }
      finally
      {
        CleanCallContext();
      }
    }

    /// <summary>
    /// Метод для обработки полученного сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    protected abstract Task ProcessReceivedMessage(BaseMessage message);

    private async Task ReconnectAfterErrorAsync(CancellationToken cancellationToken)
    {
      Interlocked.Increment(ref _errorCount);

      await Task.Delay(CalculateTimeout(), cancellationToken);
      NotifyNeedReconnect();
      ReconnectToQueue();
    }

    private int CalculateTimeout()
    {
      return InternalTools.CalculateTimeout(_errorCount, ErrorThresholdCount, DefaultReconnectTimeout, MaxReconnectTimeout);
    }

    private void ReconnectToQueue()
    {
      try
      {
        _adapter.Disconnect();
        _adapter.Connect();
      }
      catch (Exception e)
      {
        Logger.LogError(e, "Error during with reconnect to queue");
      }
    }

    private void SetCallContextFromMessage(BaseMessage message)
    {
      var context = MessagingContext.Create(message);

      Logger.RequestId = context.RequestId;
      Logger.User = context.UserId;

      _messagingCallContextAccessor.SetContext(context);
    }

    private void CleanCallContext()
    {
      _messagingCallContextAccessor.CleanContext();
    }

    /// <inheritdoc />
    public void Dispose()
    {
      if (_isDisposed)
      {
        return;
      }

      try
      {
        Stop();
        _adapter.Dispose();
      }
      catch (Exception e)
      {
        Logger.LogError(e, "Error while dispose");
      }
      finally
      {
        _isDisposed = true;
      }
    }

    ~QueuePollingReactorBase() => Dispose();
  }
}
