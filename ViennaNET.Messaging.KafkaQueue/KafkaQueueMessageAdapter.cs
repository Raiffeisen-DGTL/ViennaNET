using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.KafkaQueue
{
  /// <summary>
  ///   Предоставляет функционал по работе с сообщениями Kafka
  /// </summary>
  internal class KafkaQueueMessageAdapter : IMessageAdapter
  {
    private readonly IKafkaConnectionFactory _connectionFactory;
    private readonly ILogger<KafkaQueueMessageAdapter> _logger;
    private readonly KafkaQueueConfiguration _configuration;
    private readonly object _connectionLock = new object();

    private bool _isDisposed;
    private IConsumer<Ignore, byte[]> _consumer;
    private IProducer<Null, byte[]> _producer;

    /// <summary>
    ///   Инициализирует экземпляр переменной типа <see cref="QueueConfigurationBase" />
    ///   и признаком необходимости проводить диагностику
    /// </summary>
    /// <param name="queueConfiguration">Конфигурация подключения</param>
    /// <param name="connectionFactory">Фабрика для создания подключения к оередям</param>
    /// <param name="logger">Интерфейс логгирования</param>
    public KafkaQueueMessageAdapter(KafkaQueueConfiguration queueConfiguration, IKafkaConnectionFactory connectionFactory, ILogger<KafkaQueueMessageAdapter> logger)
    {
      _connectionFactory = connectionFactory;
      _logger = logger;
      _configuration = queueConfiguration.ThrowIfNull(nameof(queueConfiguration));
    }

    /// <inheritdoc />
    public bool IsConnected => !_isDisposed && (_consumer != null || _producer != null);

    /// <inheritdoc />
    public QueueConfigurationBase Configuration => _configuration;

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
          if (_configuration.IsConsumer)
          {
            _consumer = _connectionFactory.CreateConsumer(_configuration, ConsumerLogHandler, ConsumerErrorHandler);
            _consumer.Subscribe(_configuration.QueueName);
          }
          else
          {
            _producer = _connectionFactory.CreateProducer(_configuration, ProducerLogHandler, ProducerErrorHandler);
          }

          _logger.LogDebug("Done to connection to queue with id: {queueId}", _configuration.Id);
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
      if (_isDisposed)
      {
        return;
      }

      _consumer?.Dispose();
      _consumer = null;
      _producer?.Dispose();
      _producer = null;
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
        _producer.Produce(_configuration.QueueName, Prepare(message));
        _logger.LogDebug("Done to send message to queue with id: {queueId}", _configuration.Id);
        return message;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Kafka PUT failure on queue with ID: {queueId}", _configuration.Id);
        throw new MessagingException(ex, "Messaging error while sending message. See inner exception for more details");
      }
    }

    /// <inheritdoc />
    public BaseMessage Receive(
      string correlationId = null, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      var hasMessage = TryReceive(out var message, correlationId, timeout, additionalParameters);
      return hasMessage ? message : throw new MessageDidNotReceivedException("Can not receive message because queue is empty");
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
        var dataResult = _consumer.Consume();
        if (dataResult == null)
        {
          return false;
        }

        message = new BytesMessage
        {
          Body = dataResult.Message.Value,
          ReceiveDate = DateTime.Now,
          SendDateTime = dataResult.Message.Timestamp.UtcDateTime,
          ReplyQueue = dataResult.Topic
        };

        foreach (var header in dataResult.Headers)
        {
          message.Properties.Add(header.Key, Encoding.UTF8.GetString(header.GetValueBytes()));
        }

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
      return processingType == MessageProcessingType.ThreadStrategy;
    }

    [ExcludeFromCodeCoverage]
    private void ProducerErrorHandler(IProducer<Null, byte[]> producer, Error error)
    {
      _logger.LogError(
        "Producer log for queue {queueId}. Code: {errorCode}, isBrokerError: {isBrokerError}, isError: {isError}, " + 
        "isFatal: {isFatal}, isLocalError: {isLocalError}, reason: {errorReason}", 
        _configuration.Id,
        error.Code,
        error.IsBrokerError,
        error.IsError,
        error.IsFatal,
        error.IsLocalError,
        error.Reason);
    }

    [ExcludeFromCodeCoverage]
    private void ProducerLogHandler(IProducer<Null, byte[]> producer, LogMessage log)
    {
      _logger.LogDebug(
        "Producer log for queue {queueId}. Level: {logLevel}, name: {logName}, message: {message}, facility: {facility}",
        _configuration.Id,
        log.Level,
        log.Name,
        log.Message,
        log.Facility);
    }

    [ExcludeFromCodeCoverage]
    private void ConsumerErrorHandler(IConsumer<Ignore, byte[]> producer, Error error)
    {
      _logger.LogError(
        "Consumer log for queue {queueId}. Code: {errorCode}, isBrokerError: {isBrokerError}, isError: {isError}, " +
        "isFatal: {isFatal}, isLocalError: {isLocalError}, reason: {errorReason}",
        _configuration.Id,
        error.Code,
        error.IsBrokerError,
        error.IsError,
        error.IsFatal,
        error.IsLocalError,
        error.Reason);
    }

    [ExcludeFromCodeCoverage]
    private void ConsumerLogHandler(IConsumer<Ignore, byte[]> consumer, LogMessage log)
    {
      _logger.LogDebug(
        "Consumer log for queue {queueId}. Level: {logLevel}, name: {logName}, message: {message}, facility: {facility}",
        _configuration.Id,
        log.Level,
        log.Name,
        log.Message,
        log.Facility);
    }

    private void CheckDisposed()
    {
      if (_isDisposed)
      {
        throw new ObjectDisposedException(nameof(KafkaQueueMessageAdapter));
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

    private Message<Null, byte[]> Prepare(BaseMessage message)
    {
      if (string.IsNullOrWhiteSpace(message.MessageId))
      {
        message.MessageId = Guid.NewGuid()
                                .ToString()
                                .ToUpper();
      }

      message.SendDateTime = DateTime.Now;

      var headers = new Headers();
      foreach (var property in message.Properties)
      {
        if (!(property.Value is string str))
        {
          throw new
            MessagingException($"The value of property {property.Key} is not a string. Kafka adapter support only string properties");
        }

        headers.Add(property.Key, Encoding.UTF8.GetBytes(str));
      }

      LogMessageInternal(message, true);
      return new Message<Null, byte[]>
      {
        Timestamp = new Timestamp(message.SendDateTime.GetValueOrDefault()), Headers = headers, Value = message.GetMessageBodyAsBytes()
      };
    }

    private void LogMessageInternal(BaseMessage message, bool isSend)
    {
      _logger.LogDebug(
        $"Message has been {(isSend ? "sent to" : "received from")} queue with ID:{{queueId}}{Environment.NewLine}{{message}}",
        _configuration.Id,
        message);
    }
  }
}