using System;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Diagnostic;
using ViennaNET.Logging;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Processing.Impl.Subscribe
{
  /// <inheritdoc />
  public abstract class QueueSubscribedReactorBase : IQueueReactor
  {
    private readonly IMessagingCallContextAccessor _messagingCallContextAccessor;
    private readonly IHealthCheckingService _healthCheckingService;

    private readonly bool _serviceHealthDependent;
    private readonly int _reconnectTimeout;
    private readonly string _pollingId;

    private Polling _reconnectPolling;

    /// <summary>
    ///   Адаптер для взаимодействия с очередью
    /// </summary>
    protected readonly IMessageAdapterWithSubscribing adapter;

    private int _errorCount;
    private bool _hasDiagnosticErrors;

    private bool _isDisposed;

    /// <summary>
    ///   Базовый класс для реактора, работающего на основе шаблона "Наблюдатель"
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected QueueSubscribedReactorBase(
      IMessageAdapterWithSubscribing messageAdapter, int reconnectTimeout, string pollingId, bool? serviceHealthDependent,
      IHealthCheckingService healthCheckingService, IMessagingCallContextAccessor messagingCallContextAccessor)
    {
      adapter = messageAdapter.ThrowIfNull(nameof(messageAdapter));
      _messagingCallContextAccessor = messagingCallContextAccessor.ThrowIfNull(nameof(messagingCallContextAccessor));
      _healthCheckingService = healthCheckingService.ThrowIfNull(nameof(serviceHealthDependent));
      if (reconnectTimeout <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(reconnectTimeout));
      }

      _hasDiagnosticErrors = false;
      _serviceHealthDependent = serviceHealthDependent ?? false;

      _reconnectTimeout = reconnectTimeout;
      _pollingId = pollingId;
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

      _reconnectPolling = new Polling(_reconnectTimeout, CheckAndReconnect, _pollingId);
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
        Logger.LogError(e, "Error while dispose");
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
        Logger.LogDebug("Diagnostic not passed. Skip listening");
        return (true, true);
      }

      if (adapter.IsConnected)
      {
        Logger.LogWarning($"Adapter with queue id {adapter.Configuration.Id} already connected");
        return (true, false);
      }

      try
      {
        adapter.Connect();
        adapter.Subscribe(ProcessMessageAsync);
      }
      catch (TimeoutException ex)
      {
        Logger.LogError(ex, $"Cannot connect to queue with id {adapter.Configuration.Id}");
        return (true, true);
      }
      catch(Exception ex)
      {
        Logger.LogError(ex, $"Cannot connect to queue with id {adapter.Configuration.Id}");
        return (true, false);
      }

      return (false, false);
    }

    private void OnDiagnosticFailed()
    {
      _hasDiagnosticErrors = true;
      Unsubscribe();
      Logger.LogDebug("QueueSubscribedReactor: Service diagnostic failed, stop listening");
    }

    private void OnDiagnosticPassed()
    {
      _hasDiagnosticErrors = false;
      Logger.LogDebug("QueueSubscribedReactor: Service diagnostic passed");
    }

    private Task<bool> CheckAndReconnect(CancellationToken cancellationToken)
    {
      try
      {
        Logger.LogDebug($"'{nameof(CheckAndReconnect)}' invocation started");
        StartProcessingInternal();
        Logger.LogDebug($"'{nameof(CheckAndReconnect)}' invocation finished");
      }
      catch (Exception exception)
      {
        Logger.LogError(exception, "QueueSubscribedReactor: CheckAndReconnect failed");
      }

      return Task.FromResult(false);
    }

    private void Unsubscribe()
    {
      adapter.Unsubscribe();
      adapter.Disconnect();
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
        Logger.LogDebug($"Message has been received by subscribing {Environment.NewLine} {message}");
        SetCallContextFromMessage(message);

        var processed = await GetProcessedMessageAsync(message);
        if (!processed)
        {
          processed = GetProcessedMessage(message);
        }

        if (!processed)
        {
          Logger.LogError($"The message did not processed. Message body: {message.LogBody()}");
        }

        Interlocked.Exchange(ref _errorCount, 0);
      }
      catch (ReplyException)
      {
        throw;
      }
      catch (SystemException exSystem)
      {
        Logger.LogError(exSystem, "Process message failed: commit message");
      }
      catch (Exception e)
      {
        Logger.LogError(e, "Error while executing process handler");
      }
      finally
      {
        CleanCallContext();
      }
    }

    ~QueueSubscribedReactorBase() => Dispose();
  }
}
