using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Exceptions;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  /// <summary>
  ///   Предоставляет функционал по работе с сообщениями RabbitMQ
  /// </summary>
  public class RabbitMqQueueMessageAdapter : IMessageAdapterWithSubscribing
  {
    private readonly ILogger<RabbitMqQueueMessageAdapter> _logger;
    private const ushort DefaultRequestedHeartbeat = 10;
    private const int DefaultReplyTimeout = 30;
    private const string ReplyExchangeName = "replyExchange";
    private const int TempQueueLifetimeAdditionMs = 10000;

    private readonly RabbitMqQueueConfiguration _configuration;
    private readonly object _connectionLock = new object();

    private bool _isDisposed;
    private IAdvancedBus _advancedBus;
    private IDisposable _consumer;
    private IExchange _exchange;
    private IQueue _queue;

    private readonly IAdvancedBusFactory _advancedBusFactory;

    /// <summary>
    ///   Инициализирует экземпляр фабрикой, создающей <see cref="IAdvancedBus" />,
    ///   переменной типа <see cref="QueueConfigurationBase" />
    ///   и признаком необходимости проводить диагностику
    /// </summary>
    /// <param name="advancedBusFactory">Фабрика для создания <see cref="IAdvancedBus" /></param>
    /// <param name="configuration">Конфигурация очереди</param>
    /// <param name="logger">Интерфейс логгирования</param>
    public RabbitMqQueueMessageAdapter(
      IAdvancedBusFactory advancedBusFactory, 
      RabbitMqQueueConfiguration configuration, 
      ILogger<RabbitMqQueueMessageAdapter> logger)
    {
      _logger = logger;
      _advancedBusFactory = advancedBusFactory;
      _configuration = configuration;
    }

    /// <inheritdoc />
    public QueueConfigurationBase Configuration => _configuration;

    /// <inheritdoc />
    public bool IsConnected => !_isDisposed && _advancedBus != null && _advancedBus.IsConnected;

    /// <inheritdoc />
    /// <summary>
    ///   Создает соединение с очередью
    /// </summary>
    public void Connect()
    {
      _logger.LogDebug("Trying to connect to queue with id: {queueId}", _configuration.Id);

      CheckDisposed();
      if (IsConnected)
      {
        _logger.LogDebug("Already connected to queue with id: {queueId}", _configuration.Id);
        return;
      }

      lock (_connectionLock)
      {
        try
        {
          _advancedBus = _advancedBusFactory.Create(_configuration.Server, _configuration.Port, _configuration.VirtualHost ?? "/",
                                                    _configuration.User, _configuration.Password, DefaultRequestedHeartbeat, x => {});

          var isQueueNameSpecified = !string.IsNullOrWhiteSpace(_configuration.QueueName);
          var isExchangeNameSpecified = !string.IsNullOrWhiteSpace(_configuration.ExchangeName);

          if (isQueueNameSpecified)
          {
            _queue = _advancedBus.QueueDeclare(_configuration.QueueName);
          }

          if (isExchangeNameSpecified)
          {
            var type = string.IsNullOrWhiteSpace(_configuration.ExchangeType)
              ? ExchangeType.Fanout
              : _configuration.ExchangeType;
            _exchange = _advancedBus.ExchangeDeclare(_configuration.ExchangeName, type);
          }

          if (isQueueNameSpecified && isExchangeNameSpecified)
          {
            InitializeBinds();
          }

          _logger.LogDebug("Established connection to queue with id: {queueId}", _configuration.Id);
        }
        catch (Exception e)
        {
          _logger.LogError(e, "Fail to connection to queue with id: {queueId}", _configuration.Id);
          Disconnect();
          throw;
        }
      }
    }

    private void InitializeBinds()
    {
      _advancedBus.Bind(_exchange, _queue, _configuration.Id);
      if (_configuration.Routings == null)
      {
        return;
      }

      foreach (var routing in _configuration.Routings)
      {
        _advancedBus.Bind(_exchange, _queue, routing);
      }
    }

    /// <inheritdoc />
    public void Disconnect()
    {
      if (_isDisposed)
      {
        return;
      }

      _consumer?.Dispose();
      _advancedBus?.Dispose();
    }

    /// <inheritdoc />
    public void Subscribe(Func<BaseMessage, Task> handler)
    {
      CheckDisposed();
      CheckAndReconnect();

      _logger.LogDebug("Try to subscribe to queue with id: {queueId}", _configuration.Id);
      _consumer =
        _advancedBus.Consume(_queue, (body, messageProperties, info) => handler?.Invoke(body.ConvertToBaseMessage(messageProperties)));
      _logger.LogDebug("Done to subscribe to queue with id: {queueId}", _configuration.Id);
    }

    /// <inheritdoc />
    public void Unsubscribe()
    {
      _logger.LogDebug("Try to unsubscribe to queue with id: {queueId}", _configuration.Id);
      _consumer?.Dispose();
      _logger.LogDebug("Done to unsubscribe to queue with id: {queueId}", _configuration.Id);
    }

    /// <inheritdoc />
    /// <summary>
    ///   Отправляет сообщение в очередь
    /// </summary>
    /// <returns>Отправленное сообщение с заполнеными датой отправки и идентификатором сообщения</returns>
    public BaseMessage Send(BaseMessage message)
    {
      _logger.LogDebug("Try to send message to queue with id: {queueId}", _configuration.Id);
      message.ThrowIfNull(nameof(message));
      CheckDisposed();
      CheckAndReconnect();
      try
      {
        var body = message.GetMessageBodyAsBytes();
        _advancedBus.Publish(_exchange, _configuration.Id, false, message.ConvertToProperties(_configuration), body);
        _logger.LogDebug("Done to send message to queue with id: {queueId}", _configuration.Id);
        LogMessageInternal(message, true);
        return message;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "RabbitMQ PUT failure on queue with ID: {queueId}", _configuration.Id);
        throw new MessagingException(ex, "Messaging error while sending message. See inner exception for more details");
      }
    }

    /// <inheritdoc />
    public BaseMessage Receive(
      string correlationId = null, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      var hasMessage = TryReceive(out var message, correlationId, timeout, additionalParameters);
      return hasMessage
        ? message
        : throw new MessageDidNotReceivedException("Can not receive message because queue is empty");
    }

    /// <inheritdoc />
    public void Dispose()
    {
      Disconnect();
      _isDisposed = true;
      GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public bool TryReceive(
      out BaseMessage message, string correlationId = null, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters)
    {
      message = null;
      _logger.LogDebug("Try to receive message from queue with id: {queueId}", _configuration.Id);

      CheckDisposed();
      CheckAndReconnect();

      try
      {
        var dataResult = _advancedBus.Get(_queue);
        if (dataResult == null)
        {
          return false;
        }

        message = dataResult.Body.ConvertToBaseMessage(dataResult.Properties);

        LogMessageInternal(message, false);
        return true;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "Receive message failure on queue with ID: {queueId}", _configuration.Id);
        message = null;
        return false;
      }
    }

    /// <inheritdoc />
    public bool SupportProcessingType(MessageProcessingType processingType)
    {
      return processingType == MessageProcessingType.Subscribe || processingType == MessageProcessingType.SubscribeAndReply;
    }

    /// <inheritdoc />
    public async Task<BaseMessage> RequestAndWaitResponse(BaseMessage message)
    {
      _logger.LogDebug("Try to send message to queue with id: {queueId} and wait response", _configuration.Id);
      message.ThrowIfNull(nameof(message));
      CheckDisposed();
      CheckAndReconnect();

      var tcs = new TaskCompletionSource<BaseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

      try
      {
        var replyQueueName = $"reply.{_configuration.QueueName}.{Guid.NewGuid()}";
        message.ReplyQueue = replyQueueName;
        message.LifeTime = TimeSpan.FromSeconds(_configuration.ReplyTimeout ?? DefaultReplyTimeout);

        lock (_connectionLock)
        {
          var replyExchange = _advancedBus.ExchangeDeclare(ReplyExchangeName, ExchangeType.Direct);
          var replyQueue = _advancedBus.QueueDeclare(replyQueueName, durable: false, autoDelete: true,
                                                     expires: (_configuration.ReplyTimeout ?? DefaultReplyTimeout) * 1000
                                                              + TempQueueLifetimeAdditionMs);
          _advancedBus.Bind(replyExchange, replyQueue, replyQueueName);
          _advancedBus.Consume(replyQueue, (body, messageProperties, info) => Task.Factory.StartNew(() =>
          {
            tcs.SetResult(body.ConvertToBaseMessage(messageProperties));
          }));
        }

        var timer = new Timer(state =>
        {
          if (!tcs.Task.IsCompleted)
          {
            tcs.TrySetException(new TimeoutException($"Request timed out. ReplyQueueName: {replyQueueName}"));
          }
        }, null, TimeSpan.FromSeconds(_configuration.ReplyTimeout ?? DefaultReplyTimeout), TimeSpan.FromMilliseconds(-1.0));

        using (timer)
        {
          Send(message);
          var result = await tcs.Task;
          return result;
        }
      }
      catch (Exception ex)
      {
        throw new MessagingException(ex, "Messaging error while sending reply message. See inner exception for more details");
      }
    }

    /// <inheritdoc />
    public BaseMessage Reply(BaseMessage message)
    {
      _logger.LogDebug("Try to send reply message to queue with id: {queueId}", _configuration.Id);
      message.ThrowIfNull(nameof(message));
      CheckDisposed();
      CheckAndReconnect();

      try
      {
        var replyExchange = _advancedBus.ExchangeDeclare(ReplyExchangeName, ExchangeType.Direct);
        var queue = _advancedBus.QueueDeclare(message.ReplyQueue, true);
        _advancedBus.Bind(replyExchange, queue, message.ReplyQueue);
        var body = message.GetMessageBodyAsBytes();
        _advancedBus.Publish(replyExchange, message.ReplyQueue, false, message.ConvertToProperties(_configuration), body);
        _logger.LogDebug("Done sending reply message to queue with id: {queueId}", message.ReplyQueue);
        LogMessageInternal(message, true);
        return message;
      }
      catch (OperationInterruptedException e)
      {
        _logger.LogError(e,"RabbitMQ PUT reply failure by OperationInterruptedException on queue with ID: {queueId}", _configuration.Id);
        throw new ReplyException(e, "Messaging error while sending reply message. See inner exception for more details");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex,"RabbitMQ PUT reply failure on queue with ID: {queueId}", _configuration.Id);
        throw new MessagingException(ex, "Messaging error while sending reply message. See inner exception for more details");
      }
    }

    private void CheckDisposed()
    {
      if (_isDisposed)
      {
        throw new ObjectDisposedException(nameof(RabbitMqQueueMessageAdapter));
      }
    }

    private void CheckAndReconnect()
    {
      var isConnected = IsConnected;
      _logger.LogDebug("IsConnected: {isConnected}, queue with ID: {queueId}", isConnected, _configuration.Id);
      if (!isConnected)
      {
        Connect();
      }
    }

    private void LogMessageInternal(BaseMessage message, bool isSend)
    {
      _logger.LogDebug(
        $"Message has been {(isSend ? "sent to" : "received from")} queue with ID:{{queueId}}{Environment.NewLine}{{message}}",
        _configuration.Id,
        message?.LogBody());
    }
  }
}