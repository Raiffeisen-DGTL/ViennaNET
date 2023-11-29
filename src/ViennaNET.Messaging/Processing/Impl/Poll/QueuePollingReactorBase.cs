﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Processing.Impl.Poll
{
  /// <inheritdoc />
  public abstract class QueuePollingReactorBase : IQueueReactor
  {
    private const int ErrorThresholdCount = 15;
    private const int DefaultReconnectTimeout = 600;
    private const int MaxReconnectTimeout = 60000;

    private readonly IMessageAdapter _adapter;
    private readonly IEnumerable<IMessageProcessorAsync> _asyncMessageProcessors;
    private readonly IHealthCheckingService _healthCheckingService;
    private readonly ILogger _logger;
    private readonly IEnumerable<IMessageProcessor> _messageProcessors;

    private readonly IMessagingCallContextAccessor _messagingCallContextAccessor;
    private readonly bool _serviceHealthDependent;
    private readonly int _subscribeInterval;
    private int _errorCount;
    private bool _hasDiagnosticErrors;
    private bool _isDisposed;
    private IDisposable _loggerContext;

    private Polling _subscribePolling;

    /// <summary>
    ///   Инициализирует компонент ссылками на <see cref="IMessageAdapter" />, <see cref="IHealthCheckingService" />,
    ///   коллекции <see cref="IMessageProcessor" />, <see cref="IMessageProcessorAsync" />
    /// </summary>
    /// <param name="messageAdapter">Адаптер для работы с очередью</param>
    /// <param name="messageProcessors">Процессоры для обработки сообщения</param>
    /// <param name="asyncMessageProcessors">Асинхронные процессоры для обработки сообщения</param>
    /// <param name="subscribeInterval">Интервал подписки</param>
    /// <param name="serviceHealthDependent">Признак использования диагностики</param>
    /// <param name="healthCheckingService">Ссылка на службу диагностики</param>
    /// <param name="messagingCallContextAccessor">Контейнер с параметрами вызова для сообщения</param>
    /// <param name="logger">Интерфейс логгирования</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected QueuePollingReactorBase(
      IMessageAdapter messageAdapter,
      IEnumerable<IMessageProcessor> messageProcessors,
      IEnumerable<IMessageProcessorAsync> asyncMessageProcessors,
      int subscribeInterval,
      bool? serviceHealthDependent,
      IHealthCheckingService healthCheckingService,
      IMessagingCallContextAccessor messagingCallContextAccessor,
      ILogger logger)
    {
      _adapter = messageAdapter;
      _messageProcessors = messageProcessors;
      _asyncMessageProcessors = asyncMessageProcessors;
      if (subscribeInterval <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(subscribeInterval));
      }

      _healthCheckingService = healthCheckingService;
      _hasDiagnosticErrors = false;
      _serviceHealthDependent = serviceHealthDependent ?? false;

      _messagingCallContextAccessor = messagingCallContextAccessor;
      _logger = logger;

      _subscribeInterval = subscribeInterval;
    }

    /// <inheritdoc />
    public bool StartProcessing()
    {
      if (_subscribePolling != null && _subscribePolling.IsStarted)
      {
        return false;
      }

      if (_serviceHealthDependent)
      {
        _healthCheckingService.DiagnosticPassedEvent += OnDiagnosticPassed;
        _healthCheckingService.DiagnosticFailedEvent += OnDiagnosticFailed;
      }

      _subscribePolling = new Polling(0, ListenMessagesAsync, _logger);
      _subscribePolling.StartPolling();
      return _subscribePolling.IsStarted;
    }

    /// <inheritdoc />
    public void Stop()
    {
      try
      {
        if (_serviceHealthDependent)
        {
          _healthCheckingService.DiagnosticFailedEvent -= OnDiagnosticFailed;
          _healthCheckingService.DiagnosticPassedEvent -= OnDiagnosticPassed;
        }

        _subscribePolling?.Dispose();
        _subscribePolling = null;
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
        _logger.LogError(e, "Error while dispose");
      }
      finally
      {
        _isDisposed = true;
      }
    }

    private void OnDiagnosticFailed()
    {
      _hasDiagnosticErrors = true;
      _logger.LogDebug("QueuePollingReactor: Service diagnostic failed, stop listening");
    }

    private void OnDiagnosticPassed()
    {
      _hasDiagnosticErrors = false;
      _logger.LogDebug("QueuePollingReactor: Service diagnostic passed");
    }

    /// <summary>
    ///   Отправляет сообщение в цепочку процессоров для последующей обработки в асинхронном режиме
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>Признак успешности обработки</returns>
    protected bool GetProcessedMessage(BaseMessage message)
    {
      return _messageProcessors.Any(processor => processor.Process(message));
    }

    /// <summary>
    ///   Отправляет сообщение в цепочку процессоров для последующей обработки
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
        _logger.LogError(e, "Error while execute reconnect event");
      }
    }

    private async Task<bool> ListenMessagesAsync(CancellationToken cancellationToken)
    {
      try
      {
        _logger.LogDebug("Listen messages ...");

        if (_serviceHealthDependent && _hasDiagnosticErrors)
        {
          _logger.LogDebug("Diagnostic not passed. Skip listening");
          return false;
        }

        if (!_adapter.IsConnected)
        {
          _adapter.Connect();
        }

        if (_adapter.TryReceive(out var message, timeout: TimeSpan.FromMilliseconds(_subscribeInterval)))
        {
          SetCallContextFromMessage(message);
          await ProcessReceivedMessage(message);
        }

        Interlocked.Exchange(ref _errorCount, 0);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error during message processing");
        await ReconnectAfterErrorAsync(cancellationToken);
      }
      finally
      {
        CleanCallContext();
      }

      return true;
    }

    /// <summary>
    ///   Метод для обработки полученного сообщения
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
      return InternalTools.CalculateTimeout(_errorCount, ErrorThresholdCount, DefaultReconnectTimeout,
        MaxReconnectTimeout);
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
        _logger.LogError(e, "Error during with reconnect to queue");
      }
    }

    private void SetCallContextFromMessage(BaseMessage message)
    {
      var context = MessagingContext.Create(message);

      _loggerContext =
        _logger.BeginScope("RequestID: {requestId}, UserID: {userId}", context.RequestId, context.UserId);

      _messagingCallContextAccessor.SetContext(context);
    }

    private void CleanCallContext()
    {
      _messagingCallContextAccessor.CleanContext();

      _loggerContext?.Dispose();
      _loggerContext = null;
    }

    ~QueuePollingReactorBase()
    {
      Dispose();
    }
  }
}