using System;
using System.Text;
using Confluent.Kafka;
using ViennaNET.Logging;
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
  public class KafkaQueueMessageAdapter : IMessageAdapter
  {
    private readonly KafkaQueueConfiguration _configuration;
    private readonly object _connectionLock;
    private readonly bool _isDiagnostic;
    private readonly bool _isDisposed;

    private IConsumer<Ignore, byte[]> _consumer;
    private IProducer<Null, byte[]> _producer;

    /// <summary>
    ///   Инициализирует экземпляр переменной типа <see cref="QueueConfigurationBase" />
    ///   и признаком необходимости проводить диагностику
    /// </summary>
    /// <param name="queueConfiguration"></param>
    /// <param name="isDiagnostic"></param>
    public KafkaQueueMessageAdapter(KafkaQueueConfiguration queueConfiguration, bool isDiagnostic)
    {
      _configuration = queueConfiguration.ThrowIfNull(nameof(queueConfiguration));
      _connectionLock = new object();
      _isDisposed = false;
      _isDiagnostic = isDiagnostic;
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
          if (_configuration.IsConsumer)
          {
            _consumer = new ConsumerBuilder<Ignore, byte[]>(new ConsumerConfig
              {
                GroupId = _configuration.GroupId,
                BootstrapServers = $"{_configuration.Server}",
                AutoOffsetReset = _configuration.AutoOffsetReset,
                SaslKerberosKeytab = _configuration.KeyTab,
                SaslKerberosPrincipal = _configuration.User,
                SaslKerberosServiceName = _configuration.ServiceName,
                SecurityProtocol = _configuration.Protocol,
                SaslMechanism = _configuration.Mechanism,
                Debug = _configuration.Debug
              }).SetLogHandler(ConsumerLogHandler)
                .SetErrorHandler(ConsumerErrorHandler)
                .Build();

            _consumer.Subscribe(_configuration.QueueName);
          }
          else
          {
            _producer = new ProducerBuilder<Null, byte[]>(new ProducerConfig
              {
                BootstrapServers = $"{_configuration.Server}",
                SaslKerberosKeytab = _configuration.KeyTab,
                SaslKerberosPrincipal = _configuration.User,
                SaslKerberosServiceName = _configuration.ServiceName,
                SecurityProtocol = _configuration.Protocol,
                SaslMechanism = _configuration.Mechanism,
                Debug = _configuration.Debug
              }).SetLogHandler(ProducerLogHandler)
                .SetErrorHandler(ProducerErrorHandler)
                .Build();
          }

          if (_isDiagnostic)
          {
            Logger.LogDiagnostic($"Trying to connect to queue with id: {_configuration.Id}");
          }
          else
          {
            Logger.LogDebug($"Done to connection to queue with id: {_configuration.Id}");
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

    /// <inheritdoc />
    public void Disconnect()
    {
      if (_isDisposed)
      {
        return;
      }

      _consumer?.Dispose();
      _producer?.Dispose();
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
        _producer.Produce(_configuration.QueueName, Prepare(message));
        Logger.LogDebug($"Done to send message to queue with id: {_configuration.Id}");
        return message;
      }
      catch (Exception ex)
      {
        Logger.LogDebug("Kafka PUT failure:");
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
        var dataResult = _consumer.Consume();
        if (dataResult == null)
        {
          Logger.LogDebugFormat("No messages available");
          throw new MessageDidNotReceivedException("Can not receive message because queue is empty");
        }

        var message = new BytesMessage
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
        return message;
      }
      catch (Exception ex)
      {
        Logger.LogError("Kafka GET failure:", ex.Message);
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
      return processingType == MessageProcessingType.ThreadStrategy;
    }

    private void ProducerErrorHandler(IProducer<Null, byte[]> producer, Error error)
    {
      Logger.LogError($"Producer log for queue {_configuration.Id}. Code: {error.Code}, isBrokerError: {error.IsBrokerError}, "
                      + $"isError: {error.IsError}, isFatal: {error.IsFatal}, isLocalError: {error.IsLocalError}, reason: {error.Reason}");
    }

    private void ProducerLogHandler(IProducer<Null, byte[]> producer, LogMessage log)
    {
      Logger.LogDebug($"Producer log for queue {_configuration.Id}. Level: {log.Level}, name: {log.Name}, "
                      + $"message: {log.Message}, facility: {log.Facility}");
    }

    private void ConsumerErrorHandler(IConsumer<Ignore, byte[]> producer, Error error)
    {
      Logger.LogError($"Consumer log for queue {_configuration.Id}. Code: {error.Code}, isBrokerError: {error.IsBrokerError}, "
                      + $"isError: {error.IsError}, isFatal: {error.IsFatal}, isLocalError: {error.IsLocalError}, reason: {error.Reason}");
    }

    private void ConsumerLogHandler(IConsumer<Ignore, byte[]> consumer, LogMessage log)
    {
      Logger.LogDebug($"Consumer log for queue {_configuration.Id}. Level: {log.Level}, name: {log.Name}, "
                      + $"message: {log.Message}, facility: {log.Facility}");
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
      Logger.LogDebug($"KafkaQueueMessageAdapter: CheckAndReconnect - IsConnected: {isConnected}");
      if (!isConnected)
      {
        Connect();
      }
    }

    private static Message<Null, byte[]> Prepare(BaseMessage message)
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

    private static void LogMessageInternal(BaseMessage message, bool isSend)
    {
      Logger.LogDebug((isSend
                        ? "Message has been sent"
                        : "Message has been received") + Environment.NewLine + message);
    }
  }
}