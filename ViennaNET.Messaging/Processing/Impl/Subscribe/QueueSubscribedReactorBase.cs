using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.CallContext;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Processing.Impl.Subscribe
{
  /// <inheritdoc />
  public abstract class QueueSubscribedReactorBase : IQueueReactor
  {
    private const int ErrorThresholdCount = 15;
    private const int DefaultReconnectTimeout = 600;
    private const int MaxReconnectTimeout = 60000;
    private readonly IHealthCheckingService _healthCheckingService;

    private readonly ILogger _logger;
    private readonly IMessagingCallContextAccessor _messagingCallContextAccessor;
    private readonly int _reconnectTimeout;

    private readonly bool _serviceHealthDependent;

    /// <summary>
    ///   Адаптер для взаимодействия с очередью
    /// </summary>
    protected readonly IMessageAdapterWithSubscribing adapter;

    private int _errorCount;
    private bool _hasDiagnosticErrors;

    private bool _isDisposed;

    private Polling _reconnectPolling;

    /// <summary>
    ///   Базовый класс для реактора, работающего на основе шаблона "Наблюдатель"
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected QueueSubscribedReactorBase(
      IMessageAdapterWithSubscribing messageAdapter,
      int reconnectTimeout,
      bool? serviceHealthDependent,
      IHealthCheckingService healthCheckingService,
      IMessagingCallContextAccessor messagingCallContextAccessor,
      ILogger logger)
    {
      _logger = logger;
      adapter = messageAdapter;
      _messagingCallContextAccessor = messagingCallContextAccessor;
      _healthCheckingService = healthCheckingService;
      if (reconnectTimeout <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(reconnectTimeout));
      }

      _hasDiagnosticErrors = false;
      _serviceHealthDependent = serviceHealthDependent ?? false;

      _reconnectTimeout = reconnectTimeout;
    }

    /// <inheritdoc />
    public bool StartProcessing()
    {
      if (_serviceHealthDependent)
      {
        _healthCheckingService.DiagnosticPassedEvent += OnDiagnosticPassed;
        _healthCheckingService.DiagnosticFailedEvent += OnDiagnosticFailed;
      }

      (bool isFailed, bool needRetry) subscribeResult;
      while (true)
      {
        subscribeResult = StartProcessingInternal();
        if (subscribeResult.isFailed && subscribeResult.needRetry)
        {
          Thread.Sleep(_reconnectTimeout);
        }
        else
        {
          break;
        }
      }

      if (subscribeResult.isFailed)
      {
        return false;
      }

      _reconnectPolling = new Polling(_reconnectTimeout, CheckAndReconnect, _logger);
      _reconnectPolling.StartPolling();

      return true;
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

        _reconnectPolling.Dispose();
        _reconnectPolling = null;
      }
      finally
      {
        Unsubscribe();
      }
    }

    /// <inheritdoc />
    public event EventHandler<EventArgs> NeedReconnect = delegate { };

    /// <summary>
    ///   Количество ошибок
    /// </summary>
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
        adapter.Dispose();
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

    private (bool isFailed, bool needRetry) StartProcessingInternal()
    {
      if (_serviceHealthDependent && _hasDiagnosticErrors)
      {
        _logger.LogDebug("Diagnostic not passed. Skip listening");
        return (true, true);
      }

      if (adapter.IsConnected)
      {
        _logger.LogWarning("Adapter with queue id {queueId} already connected", adapter.Configuration.Id);
        return (true, false);
      }

      try
      {
        adapter.Connect();
        adapter.Subscribe(ProcessMessageAsync);
      }
      catch (TimeoutException ex)
      {
        _logger.LogError(ex, "Cannot connect to queue with id {queueId}", adapter.Configuration.Id);
        return (true, true);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Cannot connect to queue with id {queueId}", adapter.Configuration.Id);
        return (true, false);
      }

      return (false, false);
    }

    private void OnDiagnosticFailed()
    {
      _hasDiagnosticErrors = true;
      Unsubscribe();
      _logger.LogDebug("QueueSubscribedReactor: Service diagnostic failed, stop listening");
    }

    private void OnDiagnosticPassed()
    {
      _hasDiagnosticErrors = false;
      _logger.LogDebug("QueueSubscribedReactor: Service diagnostic passed");
    }

    private async Task<bool> CheckAndReconnect(CancellationToken cancellationToken)
    {
      if (adapter.IsConnected)
      {
        return false;
      }

      Interlocked.Increment(ref _errorCount);
      await Task.Delay(CalculateTimeout(), cancellationToken).ConfigureAwait(false);

      try
      {
        StartProcessingInternal();
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "Failed reconnecting");
      }

      return false;
    }

    private int CalculateTimeout()
    {
      return InternalTools.CalculateTimeout(_errorCount, ErrorThresholdCount, DefaultReconnectTimeout,
        MaxReconnectTimeout);
    }

    private void Unsubscribe()
    {
      adapter.Unsubscribe();
      adapter.Disconnect();
    }

    private MessagingContext SetCallContextFromMessage(BaseMessage message)
    {
      var context = MessagingContext.Create(message);

      _messagingCallContextAccessor.SetContext(context);

      return context;
    }

    private void CleanCallContext()
    {
      try
      {
        _messagingCallContextAccessor.CleanContext();
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "Error while cleaning call context");
      }
    }

    private IDisposable GetLoggerContextFromCallContext(ICallContext context) =>
      _logger.BeginScope("RequestID: {requestId}, UserID: {userId}", context.RequestId, context.UserId);

    /// <summary>
    ///   Результат выполнения сообщения обработчиком
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected abstract bool GetProcessedMessage(BaseMessage message);

    /// <summary>
    ///   Асинхронный результат выполнения сообщения обработчиком
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected abstract Task<bool> GetProcessedMessageAsync(BaseMessage message);

    private async Task ProcessMessageAsync(BaseMessage message)
    {
      try
      {
        _logger.LogDebug("Message has been received by subscribing " + Environment.NewLine + " {message}",
          message.LogBody());

        var context = SetCallContextFromMessage(message);
        using var _ = GetLoggerContextFromCallContext(context);

        var processed = await GetProcessedMessageAsync(message);
        if (!processed)
        {
          processed = GetProcessedMessage(message);
        }

        if (!processed)
        {
          _logger.LogError("The message did not processed. Message body: {messageBody}", message.LogBody());
        }

        Interlocked.Exchange(ref _errorCount, 0);
      }
      catch (ReplyException)
      {
        throw;
      }
      catch (SystemException exSystem)
      {
        _logger.LogError(exSystem, "Process message failed: commit message");
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Error while executing process handler");
      }
      finally
      {
        CleanCallContext();
      }
    }

    ~QueueSubscribedReactorBase() => Dispose();
  }
}