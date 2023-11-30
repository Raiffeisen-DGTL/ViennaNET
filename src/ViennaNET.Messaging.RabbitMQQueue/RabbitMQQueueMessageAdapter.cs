using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
  internal sealed class RabbitMqQueueMessageAdapter : IMessageAdapterWithSubscribing
  {
    private const int DefaultReplyTimeoutSec = 30;
    private const string ReplyExchangeName = "replyExchange";
    private const int TempQueueLifetimeAdditionMs = 10000;

    private readonly RabbitMqQueueConfiguration _configuration;
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly object _connectionLock = new();
    private readonly ILogger<RabbitMqQueueMessageAdapter> _logger;
    private IModel? _channel;

    private IConnection? _connection;
    private AsyncEventingBasicConsumer? _consumer;
    private string? _consumerTag;
    private bool _isDisposed;

    /// <summary>
    ///   Инициализирует экземпляр
    /// </summary>
    /// <param name="connectionFactory">Фабрика для создания <see cref="IConnection" /></param>
    /// <param name="configuration">Конфигурация очереди</param>
    /// <param name="logger">Интерфейс логгирования</param>
    public RabbitMqQueueMessageAdapter(
      RabbitMqQueueConfiguration configuration,
      IRabbitMqConnectionFactory connectionFactory,
      ILogger<RabbitMqQueueMessageAdapter> logger)
    {
      _logger = logger;
      _configuration = configuration;
      _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public QueueConfigurationBase Configuration => _configuration;

    /// <inheritdoc />
    public bool IsConnected =>
      !_isDisposed && _channel is { IsOpen: true } && (_consumer is null || _consumer.IsRunning);

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
          Disconnect();
          _connection = _connectionFactory.Create(_configuration);
          _channel = _connection.CreateModel();

          var isQueueNameSpecified = !string.IsNullOrWhiteSpace(_configuration.QueueName);
          var isExchangeNameSpecified = !string.IsNullOrWhiteSpace(_configuration.ExchangeName);

          if (isQueueNameSpecified)
          {
            _channel.QueueDeclare(_configuration.QueueName, true, false, false);
          }

          if (isExchangeNameSpecified)
          {
            var type = string.IsNullOrWhiteSpace(_configuration.ExchangeType)
              ? ExchangeType.Fanout
              : _configuration.ExchangeType;
            _channel.ExchangeDeclare(_configuration.ExchangeName, type, true);
          }

          if (isQueueNameSpecified && isExchangeNameSpecified)
          {
            _channel.QueueBind(_configuration.QueueName, _configuration.ExchangeName, _configuration.Id);

            if (_configuration.Routings != null)
            {
              foreach (var routing in _configuration.Routings)
              {
                _channel.QueueBind(_configuration.QueueName, _configuration.ExchangeName, routing);
              }
            }
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

    /// <inheritdoc />
    public void Disconnect()
    {
      if (!_isDisposed)
      {
        _channel?.Dispose();
        _channel = null;

        _connection?.Dispose();
        _connection = null;
      }
    }

    /// <inheritdoc />
    public void Subscribe(Func<BaseMessage, Task> handler)
    {
      handler.ThrowIfNull(nameof(handler));
      CheckDisposed();
      CheckAndReconnect();

      _logger.LogDebug("Try to subscribe to queue with id: {queueId}", _configuration.Id);

      Unsubscribe();

      _consumer = new AsyncEventingBasicConsumer(_channel);
      if (_configuration.AutoAck)
      {
        _consumer.Received += (_, args) => handler.Invoke(args.ConvertToBaseMessage());
      }
      else
      {
        _consumer.Received += async (_, args) =>
        {
          try
          {
            await handler.Invoke(args.ConvertToBaseMessage());
            _channel.BasicAck(args.DeliveryTag, false);
          }
          catch (Exception ex)
          {
            _logger.LogError(
              ex,
              "Error when handling RabbitMQ message for queue {queueId}",
              _configuration.Id);
            _channel.BasicReject(args.DeliveryTag, _configuration.Requeue);
          }
        };
      }

      _consumer.Shutdown += (_, _) =>
      {
        _logger.LogDebug("Consumer shutdown for queue {QueueId}", _configuration.Id);
        return Task.CompletedTask;
      };

      _consumer.ConsumerCancelled += (_, _) =>
      {
        _logger.LogDebug("Consumer cancelled for queue {QueueId}", _configuration.Id);
        return Task.CompletedTask;
      };

      _consumer.Unregistered += (_, _) =>
      {
        _logger.LogDebug("Consumer unregistered for queue {QueueId}", _configuration.Id);
        return Task.CompletedTask;
      };

      _consumerTag = _channel.BasicConsume(_configuration.QueueName, _configuration.AutoAck, _consumer);
      _logger.LogDebug("Done to subscribe to queue with id: {QueueId}", _configuration.Id);
    }

    /// <inheritdoc />
    public void Unsubscribe()
    {
      _logger.LogDebug("Try to unsubscribe to queue with id: {queueId}", _configuration.Id);

      if (!string.IsNullOrEmpty(_consumerTag) && _consumer?.IsRunning is true)
      {
        _channel.BasicCancel(_consumerTag);
      }

      _consumerTag = null;
      _consumer = null;

      _logger.LogDebug("Done to unsubscribe to queue with id: {queueId}", _configuration.Id);
    }

    /// <inheritdoc />
    /// <summary>
    ///   Отправляет сообщение в очередь
    /// </summary>
    /// <returns>Отправленное сообщение с заполнеными датой отправки и идентификатором сообщения</returns>
    public BaseMessage Send(BaseMessage message)
    {
      message.ThrowIfNull(nameof(message));
      _logger.LogDebug("Try to send message to queue with id: {queueId}", _configuration.Id);

      CheckDisposed();
      CheckAndReconnect();

      try
      {
        _channel.BasicPublish(
          _configuration.ExchangeName,
          _configuration.Id,
          false,
          message.ConvertToProperties(_channel.CreateBasicProperties(), _configuration.Lifetime),
          message.GetMessageBodyAsBytes());

        _logger.LogDebug("Done to send message to queue with id: {queueId}", _configuration.Id);
        LogMessageInternal(message, true);

        return message;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "RabbitMQ PUT failure on queue with ID: {queueId}", _configuration.Id);
        throw new MessagingException(
          ex,
          $"Error sending message to queue {_configuration.Id}. See inner exception");
      }
    }

    /// <inheritdoc />
    public BaseMessage Receive(
      string correlationId = null, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      var hasMessage = TryReceive(out var message, correlationId, timeout, additionalParameters);
      return hasMessage
        ? message
        : throw new MessageDidNotReceivedException(
          $"Can not receive message because queue {_configuration.Id} is empty");
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
      out BaseMessage message,
      string correlationId = null,
      TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters)
    {
      message = null;
      _logger.LogDebug("Try to receive message from queue with id: {queueId}", _configuration.Id);

      CheckDisposed();
      CheckAndReconnect();

      try
      {
        var waitTimeout = TimeoutHelper.GetTimeout(timeout);
        if (waitTimeout == TimeoutHelper.NoWaitTimeout)
        {
          var getResult = _channel.BasicGet(_configuration.QueueName, true);
          message = getResult?.ConvertToBaseMessage();
        }
        else
        {
          message = GetWithTimeout(_configuration.QueueName, waitTimeout);
        }

        if (message == null)
        {
          return false;
        }

        LogMessageInternal(message, false);
        return true;
      }
      catch (Exception exception)
      {
        _logger.LogError(
          exception,
          "Receive message failure on queue with ID: {queueId}",
          _configuration.Id);
        message = null;
        return false;
      }
    }

    /// <inheritdoc />
    public Task<BaseMessage> RequestAndWaitResponse(BaseMessage message)
    {
      _logger.LogDebug(
        "Try to send message to queue with id: {queueId} and wait response",
        _configuration.Id);
      message.ThrowIfNull(nameof(message));

      CheckDisposed();
      CheckAndReconnect();

      try
      {
        var replyQueueName = $"reply.{_configuration.QueueName}.{Guid.NewGuid()}";
        message.ReplyQueue = replyQueueName;
        message.LifeTime = TimeSpan.FromSeconds(_configuration.ReplyTimeout ?? DefaultReplyTimeoutSec);

        lock (_connectionLock)
        {
          _channel.ExchangeDeclare(ReplyExchangeName, ExchangeType.Direct, true);
          _channel.QueueDeclare(
            replyQueueName,
            false,
            false,
            true,
            new Dictionary<string, object>
            {
              {
                "x-expires",
                ((_configuration.ReplyTimeout ?? DefaultReplyTimeoutSec) * 1000) + TempQueueLifetimeAdditionMs
              }
            });
          _channel.QueueBind(replyQueueName, ReplyExchangeName, replyQueueName);
        }

        Send(message);
        var reply = GetWithTimeout(
          replyQueueName,
          (_configuration.ReplyTimeout ?? DefaultReplyTimeoutSec) * 1000);
        return reply == null
          ? throw new TimeoutException($"Request timed out. ReplyQueueName: {replyQueueName}")
          : Task.FromResult(reply);
      }
      catch (Exception ex)
      {
        throw new MessagingException(ex,
          $"Error sending reply message for queue {_configuration.Id}. See inner exception");
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
        using var replyChannel = _connection.CreateModel();
        replyChannel.ExchangeDeclarePassive(ReplyExchangeName);
        replyChannel.QueueDeclarePassive(message.ReplyQueue);

        replyChannel.BasicPublish(ReplyExchangeName,
          message.ReplyQueue,
          false,
          message.ConvertToProperties(_channel.CreateBasicProperties(), _configuration.Lifetime),
          message.GetMessageBodyAsBytes());

        _logger.LogDebug("Done sending reply message to queue with id: {queueId}", message.ReplyQueue);
        LogMessageInternal(message, true);

        return message;
      }
      catch (OperationInterruptedException e)
      {
        _logger.LogError(
          e,
          "RabbitMQ PUT reply failure by OperationInterruptedException on queue with ID: {queueId}",
          _configuration.Id);
        throw new ReplyException(
          e,
          $"Error sending reply message for queue {_configuration.Id}. See inner exception");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "RabbitMQ PUT reply failure on queue with ID: {queueId}", _configuration.Id);
        throw new MessagingException(ex,
          $"Error sending reply message for queue {_configuration.Id}. See inner exception");
      }
    }

    private BaseMessage GetWithTimeout(string queueName, long milliseconds)
    {
      var tcs = new TaskCompletionSource<BaseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

      var consumer = new AsyncEventingBasicConsumer(_channel);
      consumer.Received += (sender, args) =>
      {
        _channel.BasicCancel(args.ConsumerTag);
        tcs.SetResult(args.ConvertToBaseMessage());
        return Task.CompletedTask;
      };
      _channel.BasicConsume(queueName, true, consumer);

      var timer = new Timer(state =>
        {
          if (!tcs.Task.IsCompleted)
          {
            tcs.SetResult(null);
          }
        },
        null,
        milliseconds == TimeoutHelper.InfiniteWaitTimeout ? Timeout.Infinite : milliseconds,
        Timeout.Infinite);

      using (timer)
      {
        tcs.Task.ConfigureAwait(false);
        return tcs.Task.GetAwaiter().GetResult();
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
      _logger.LogDebug(
        "IsConnected: {isConnected}, queue with ID: {queueId}",
        isConnected,
        _configuration.Id);
      if (!isConnected)
      {
        Connect();
      }
    }

    private void LogMessageInternal(BaseMessage? message, bool isSend)
    {
      _logger.LogDebug(
        $"Message has been {(isSend ? "sent to" : "received from")} queue with ID:{{queueId}}{Environment.NewLine}{{message}}",
        _configuration.Id,
        message?.LogBody());
    }
  }
}