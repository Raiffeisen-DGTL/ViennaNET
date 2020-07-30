using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using RabbitMQ.Client.Exceptions;
using ViennaNET.Logging;
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
    private const ushort defaultRequestedHeartbeat = 10;
    private const int DefaultReplyTimeout = 30;
    private const string ReplyExchangeName = "replyExchange";
    private const int TempQueueLifetimeAdditionMs = 10000;

    private readonly RabbitMqQueueConfiguration _configuration;
    private readonly object _connectionLock;
    private readonly bool _isDiagnostic;
    private readonly bool _isDisposed;

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
    /// <param name="isDiagnostic">Признак проведения диагностики</param>
    public RabbitMqQueueMessageAdapter(IAdvancedBusFactory advancedBusFactory, RabbitMqQueueConfiguration configuration, bool isDiagnostic)
    {
      _advancedBusFactory = advancedBusFactory.ThrowIfNull(nameof(advancedBusFactory));
      _configuration = configuration.ThrowIfNull(nameof(configuration));
      _connectionLock = new object();
      _isDisposed = false;
      _isDiagnostic = isDiagnostic;
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
      if (_isDiagnostic)
      {
        Logger.LogDiagnostic($"Trying to connect to queue with id: {_configuration.Id}");
      }
      else
      {
        Logger.LogDebug($"Trying to connect to queue with id: {_configuration.Id}");
      }

      CheckDisposed();
      if (IsConnected)
      {
        Logger.LogDebug($"Already connected to queue with id: {_configuration.Id}");
        return;
      }

      lock (_connectionLock)
      {
        try
        {
          _advancedBus = _advancedBusFactory.Create(_configuration.Server, _configuration.Port, _configuration.VirtualHost ?? "/",
                                                    _configuration.User, _configuration.Password, defaultRequestedHeartbeat, x => {});

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

          if (_isDiagnostic)
          {
            Logger.LogDiagnostic($"Trying to connect to queue with id: {_configuration.Id}");
          }
          else
          {
            Logger.LogDebug($"Established connection to queue with id: {_configuration.Id}");
          }
        }
        catch (Exception e)
        {
          Logger.LogError(e, $"Fail to connection to queue with id: {_configuration.Id}");
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

      Logger.LogDebug($"Try to subscribe to queue with id: {_configuration.Id} with handler: {handler}");
      _consumer =
        _advancedBus.Consume(_queue, (body, messageProperties, info) => handler?.Invoke(body.ConvertToBaseMessage(messageProperties)));
      Logger.LogDebug($"Done to subscribe to queue with id: {_configuration.Id} with handler: {handler}");
    }

    /// <inheritdoc />
    public void Unsubscribe()
    {
      Logger.LogDebug($"Try to unsubscribe to queue with id: {_configuration.Id}");
      _consumer?.Dispose();
      Logger.LogDebug($"Done to unsubscribe to queue with id: {_configuration.Id}");
    }

    /// <inheritdoc />
    /// <summary>
    ///   Отправляет сообщение в очередь
    /// </summary>
    /// <returns>Отправленное сообщение с заполнеными датой отправки и идентификатором сообщения</returns>
    public BaseMessage Send(BaseMessage message)
    {
      Logger.LogDebug($"Try to send message to queue with id: {_configuration.Id}");
      message.ThrowIfNull(nameof(message));
      CheckDisposed();
      CheckAndReconnect();
      try
      {
        var body = message.GetMessageBodyAsBytes();
        _advancedBus.Publish(_exchange, _configuration.Id, false, message.ConvertToProperties(_configuration), body);
        Logger.LogDebug($"Done to send message to queue with id: {_configuration.Id}");
        LogMessageInternal(message, true);
        return message;
      }
      catch (Exception ex)
      {
        Logger.LogDebug("RabbitMQ PUT failure:");
        throw new MessagingException(ex, "Messaging error while sending message. See inner exception for more details");
      }
    }

    /// <inheritdoc />
    public BaseMessage Receive(
      string correlationId = null, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      Logger.LogDebug($"Try to receive message from queue with id: {_configuration.Id}");
      CheckDisposed();
      CheckAndReconnect();

      try
      {
        var dataResult = _advancedBus.Get(_queue);
        if (dataResult == null)
        {
          Logger.LogDebugFormat("No messages available");
          return null;
        }

        var message = dataResult.Body.ConvertToBaseMessage(dataResult.Properties);

        LogMessageInternal(message, false);
        return message;
      }
      catch (Exception ex)
      {
        Logger.LogError("RabbitMQ GET failure:", ex.Message);
        throw new MessagingException(ex, "Error while sending message. See inner exception for more details");
      }
    }

    /// <inheritdoc />
    public void Dispose()
    {
      Disconnect();
    }

    /// <inheritdoc />
    public bool TryReceive(
      out BaseMessage message, string correlationId = null, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters)
    {
      Logger.LogDebug($"Try to receive message from queue with id: {_configuration.Id}");
      try
      {
        message = Receive(correlationId);
        return message != null;
      }
      catch (Exception exception)
      {
        Logger.LogError(exception, "Receive message failure:");
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
      Logger.LogDebug($"Try to send message to queue with id: {_configuration.Id} and wait response");
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
      Logger.LogDebug($"Try to send reply message to queue with id: {_configuration.Id}");
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
        Logger.LogDebug($"Done to send reply message to queue with id: {message.ReplyQueue}");
        LogMessageInternal(message, true);
        return message;
      }
      catch (OperationInterruptedException e)
      {
        Logger.LogDebug("RabbitMQ PUT reply failure by OperationInterruptedException:");
        throw new ReplyException(e, "Messaging error while sending reply message. See inner exception for more details");
      }
      catch (Exception ex)
      {
        Logger.LogDebug("RabbitMQ PUT reply failure");
        throw new MessagingException(ex, "Messaging error while sending reply message. See inner exception for more details");
      }
    }

    private void CheckDisposed()
    {
      if (_isDisposed)
      {
        throw new ObjectDisposedException("Adapter is already disposed");
      }
    }

    private void CheckAndReconnect()
    {
      var isConnected = IsConnected;
      Logger.LogDebug($"RabbitMqQueueMessageAdapter: CheckAndReconnect - IsConnected: {isConnected}");
      if (!isConnected)
      {
        Connect();
      }
    }

    private static void LogMessageInternal(BaseMessage message, bool isSend)
    {
      Logger.LogDebug((isSend
                        ? "Message has been sent"
                        : "Message has been received") + Environment.NewLine + message.LogBody());
    }
  }
}