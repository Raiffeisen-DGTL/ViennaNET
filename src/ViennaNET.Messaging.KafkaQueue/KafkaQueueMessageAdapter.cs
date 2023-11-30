﻿using System;
using System.Diagnostics.CodeAnalysis;
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
    private readonly KafkaQueueConfiguration _configuration;
    private readonly IKafkaConnectionFactory _connectionFactory;
    private readonly object _connectionLock = new();
    private readonly ILogger<KafkaQueueMessageAdapter> _logger;
    protected IConsumer<byte[], byte[]>? _consumer;

    private bool _isDisposed;
    protected IProducer<byte[], byte[]>? _producer;

    /// <summary>
    ///   Инициализирует экземпляр переменной типа <see cref="QueueConfigurationBase" />
    ///   и признаком необходимости проводить диагностику
    /// </summary>
    /// <param name="queueConfiguration">Конфигурация подключения</param>
    /// <param name="connectionFactory">Фабрика для создания подключения к оередям</param>
    /// <param name="logger">Интерфейс логгирования</param>
    public KafkaQueueMessageAdapter(KafkaQueueConfiguration queueConfiguration,
      IKafkaConnectionFactory connectionFactory, ILogger<KafkaQueueMessageAdapter> logger)
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
    public virtual void Connect()
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
    public virtual void Disconnect()
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
      LogMessageInternal(message, true);

      CheckDisposed();
      CheckAndReconnect();

      try
      {
        var kafkaMessage = message.ConvertToKafkaMessage();
        _producer.Produce(_configuration.QueueName, kafkaMessage);
        _logger.LogDebug("Done to send message to queue with id: {queueId}", _configuration.Id);
        return message;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Kafka PUT failure on queue with ID: {queueId}", _configuration.Id);
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
      out BaseMessage message, string correlationId = null, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters)
    {
      message = null;
      _logger.LogDebug("Try to receive message from queue with id: {queueId}", _configuration.Id);

      CheckDisposed();
      CheckAndReconnect();

      try
      {
        ConsumeResult<byte[], byte[]> dataResult;
        var waitTimeout = TimeoutHelper.GetTimeout(timeout);
        if (waitTimeout > 0L)
        {
          dataResult = _consumer.Consume(TimeSpan.FromMilliseconds(waitTimeout));
        }
        else if (waitTimeout == TimeoutHelper.NoWaitTimeout)
        {
          dataResult = _consumer.Consume(TimeSpan.Zero);
        }
        else
        {
          dataResult = _consumer.Consume();
        }

        if (dataResult != null)
        {
          message = dataResult.ConvertToBaseMessage();
          LogMessageInternal(message, false);
          return true;
        }
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "Receive message failure on queue with ID: {queueId}", _configuration.Id);
        message = null;
      }

      return false;
    }

    [ExcludeFromCodeCoverage]
    private void ProducerErrorHandler(IProducer<byte[], byte[]> producer, Error error)
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
    private void ProducerLogHandler(IProducer<byte[], byte[]> producer, LogMessage log)
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
    private void ConsumerErrorHandler(IConsumer<byte[], byte[]> producer, Error error)
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
    private void ConsumerLogHandler(IConsumer<byte[], byte[]> consumer, LogMessage log)
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

    private void LogMessageInternal(BaseMessage? message, bool isSend)
    {
      _logger.LogDebug(
        $"Message has been {(isSend ? "sent to" : "received from")} queue with ID:{{queueId}}{Environment.NewLine}{{message}}",
        _configuration.Id,
        message?.LogBody());
    }
  }
}