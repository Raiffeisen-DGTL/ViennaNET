using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using IBM.XMS;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  /// <summary>
  ///   Базовый класс адаптеров очереди MQseries (IBM)
  /// </summary>
  internal class MqSeriesQueueMessageAdapter : IMessageAdapter
  {
    private static readonly object connectionLock = new();

    protected readonly MqSeriesQueueConfiguration _configuration;

    private readonly IMqSeriesQueueConnectionFactoryProvider _connectionFactoryProvider;
    private readonly object _consumerLock = new();
    protected readonly ILogger _logger;
    private readonly object _producerLock = new();
    private IConnection? _connection;
    private volatile IMessageConsumer? _consumer;
    private IDestination? _destination;
    private bool _isConnected;

    private bool _isDisposed;
    private volatile IMessageProducer? _producer;
    private ISession? _session;

    /// <summary>
    ///   Инициализирует адаптер
    /// </summary>
    /// <param name="connectionFactoryProvider">Объект, создающий connection factory</param>
    /// <param name="configuration">Конфигурация подключения</param>
    /// <param name="logger">Интерфейс логгирования</param>
    public MqSeriesQueueMessageAdapter(
      IMqSeriesQueueConnectionFactoryProvider connectionFactoryProvider,
      MqSeriesQueueConfiguration configuration,
      ILogger logger)
    {
      _configuration = configuration;
      _logger = logger;
      _connectionFactoryProvider = connectionFactoryProvider;
    }

    /// <inheritdoc />
    public bool IsConnected
    {
      get
      {
        ThrowIfDisposed();
        return _isConnected;
      }
    }

    /// <inheritdoc />
    public QueueConfigurationBase Configuration => _configuration;

    /// <inheritdoc />
    public void Connect()
    {
      _logger.LogDebug("Trying to connect to queue with id: {queueId}", _configuration.Id);

      ThrowIfDisposed();
      if (_isConnected)
      {
        _logger.LogDebug("Already connected to queue with id: {queueId}", _configuration.Id);
        return;
      }

      lock (connectionLock)
      {
        try
        {
          var connectionFactory = _connectionFactoryProvider.GetConnectionFactory(XMSC.CT_WMQ);
          connectionFactory.SetStringProperty(XMSC.WMQ_HOST_NAME, _configuration.Server);
          connectionFactory.SetIntProperty(XMSC.WMQ_PORT, _configuration.Port);
          connectionFactory.SetStringProperty(XMSC.WMQ_CHANNEL, _configuration.Channel);
          connectionFactory.SetIntProperty(XMSC.WMQ_CONNECTION_MODE, XMSC.WMQ_CM_CLIENT);
          connectionFactory.SetStringProperty(XMSC.WMQ_QUEUE_MANAGER, _configuration.QueueManager);
          connectionFactory.SetIntProperty(XMSC.WMQ_BROKER_VERSION, XMSC.WMQ_BROKER_DEFAULT);
          if (_configuration.TlsEnabled)
          {
            connectionFactory.SetStringProperty(XMSC.WMQ_SSL_CIPHER_SPEC, "*NEGOTIATE");
            connectionFactory.SetBooleanProperty(XMSC.WMQ_SSL_CERT_REVOCATION_CHECK, true);
            if (!string.IsNullOrWhiteSpace(_configuration.TlsClientCertLabel))
            {
              connectionFactory.SetStringProperty(XMSC.WMQ_SSL_KEY_REPOSITORY,
                _configuration.TlsClientCertStore == ClientCertificateStore.Machine ? "*SYSTEM" : "*USER");
              connectionFactory.SetStringProperty(XMSC.WMQ_SSL_CLIENT_CERT_LABEL, _configuration.TlsClientCertLabel);
            }

            if (!string.IsNullOrWhiteSpace(_configuration.TlsServerCertSubject))
            {
              connectionFactory.SetStringProperty(XMSC.WMQ_SSL_PEER_NAME, _configuration.TlsServerCertSubject);
            }
          }

          _connection = string.IsNullOrEmpty(_configuration.User)
            ? connectionFactory.CreateConnection()
            : connectionFactory.CreateConnection(_configuration.User, _configuration.Password);

          _connection.ClientID = _configuration.ClientId;

          _logger.LogDebug("Established connection to queue with id: {queueId}", _configuration.Id);
        }
        catch (XMSException ex)
        {
          DisconnectInternal();
          throw new MessagingException(
            ex,
            $"Error connecting to the queue {_configuration.Id}. See inner exception");
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error while connect to the queue with id: {queueId}", _configuration.Id);
          DisconnectInternal();
          throw;
        }
      }

      _isConnected = true;
    }

    /// <inheritdoc />
    public void Disconnect()
    {
      ThrowIfDisposed();
      try
      {
        DisconnectInternal();
      }
      catch (XMSException ex)
      {
        throw new MessagingException(
          ex,
          $"Error disconnecting from queue {_configuration.Id}. See inner exception");
      }
    }

    /// <inheritdoc />
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public BaseMessage Receive(
      string correlationId = null, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      ThrowIfDisposed();
      ThrowIfNotConnected();

      var msg = ReceiveInternal(correlationId, timeout, additionalParameters);
      return msg ?? throw new MessageDidNotReceivedException(
        $"Can not receive message because queue {_configuration.Id} is empty");
    }

    /// <inheritdoc />
    public bool TryReceive(out BaseMessage message, string correlationId = null, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters)
    {
      ThrowIfDisposed();
      ThrowIfNotConnected();

      var msg = ReceiveInternal(correlationId, timeout, additionalParameters);
      if (msg != null)
      {
        message = msg;
        return true;
      }

      message = null;
      return false;
    }

    /// <inheritdoc />
    public BaseMessage Send(BaseMessage message)
    {
      message.ThrowIfNull(nameof(message));

      ThrowIfDisposed();
      ThrowIfNotConnected();

      return SendInternal(message);
    }

    /// <inheritdoc />
    ~MqSeriesQueueMessageAdapter()
    {
      Dispose(false);
    }

    /// <summary>
    ///   Protected implementation of Dispose pattern.
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
      if (_isDisposed)
      {
        return;
      }

      if (disposing)
      {
        try
        {
          DisconnectInternal();
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error when disposing, QueueID: {queueId}", _configuration.Id);
        }
      }

      // Free any unmanaged objects here.
      //
      _isDisposed = true;
    }

    /// <summary>
    ///   Возвращает (и при необходимости создаёт) <see cref="ISession" />
    /// </summary>
    /// <returns>
    ///   <see cref="ISession" />
    /// </returns>
    protected ISession GetSession()
    {
      if (_session == null)
      {
        _session = CreateSession();
      }

      return _session;
    }

    /// <summary>
    ///   В классах-наследниках создаёт <see cref="ISession" /> с подходящими настройками
    /// </summary>
    /// <returns>
    ///   <see cref="ISession" />
    /// </returns>
    protected virtual ISession CreateSession()
    {
      return GetConnection().CreateSession(false, AcknowledgeMode.AutoAcknowledge);
    }

    /// <summary>
    ///   Возвращает <see cref="IConnection" />
    /// </summary>
    /// <returns>
    ///   <see cref="IConnection" />
    /// </returns>
    /// <exception cref="MessagingException" />
    protected IConnection GetConnection()
    {
      ThrowIfNotConnected();
      return _connection;
    }

    /// <summary>
    ///   Возвращает (и при необходимости создаёт) <see cref="IMessageConsumer" />
    /// </summary>
    /// <param name="correlationId"></param>
    /// <param name="additionalParameters"></param>
    /// <returns>
    ///   <see cref="IMessageConsumer" />
    /// </returns>
    protected IMessageConsumer GetConsumer(string correlationId = "",
      params (string Name, string Value)[] additionalParameters)
    {
      if (!string.IsNullOrWhiteSpace(correlationId))
      {
        var specialConsumer = CreateConsumer(correlationId, additionalParameters);
        GetConnection().Start();
        return specialConsumer;
      }

      if (_consumer != null)
      {
        return _consumer;
      }

      lock (_consumerLock)
      {
        if (_consumer != null)
        {
          return _consumer;
        }

        _consumer = CreateConsumer(correlationId, additionalParameters);
        GetConnection().Start();
      }

      return _consumer;
    }

    private void ThrowIfDisposed()
    {
      if (_isDisposed)
      {
        throw new ObjectDisposedException(nameof(MqSeriesQueueMessageAdapter));
      }
    }

    private void ThrowIfNotConnected()
    {
      if (!_isConnected || _connection == null)
      {
        throw new MessagingException($"Connection is not open for queue {_configuration.Id}");
      }
    }

    private void DisconnectInternal()
    {
      if (_consumer != null)
      {
        _consumer.Close();
        _consumer.Dispose();
        _consumer = null;
      }

      if (_producer != null)
      {
        _producer.Close();
        _producer.Dispose();
        _producer = null;
      }

      if (_session != null)
      {
        _session.Close();
        _session.Dispose();
        _session = null;
      }

      if (_connection != null)
      {
        _connection.Stop();
        _connection.Close();
        _connection.Dispose();
        _connection = null;
      }

      _isConnected = false;
    }

    private BaseMessage? ReceiveInternal(
      string correlationId, TimeSpan? timeout, params (string Name, string Value)[] additionalParameters)
    {
      try
      {
        _logger.LogDebug("Try to receive message from queue with id: {queueId}", _configuration.Id);

        IMessage receivedMessage;
        var consumer = GetConsumer(correlationId, additionalParameters);

        var waitTimeout = TimeoutHelper.GetTimeout(timeout);
        if (waitTimeout > 0L)
        {
          receivedMessage = consumer.Receive(waitTimeout);
        }
        else if (waitTimeout == TimeoutHelper.NoWaitTimeout)
        {
          receivedMessage = consumer.ReceiveNoWait();
        }
        else
        {
          receivedMessage = consumer.Receive();
        }

        var resultMessage = receivedMessage?.ConvertToBaseMessage();

        LogMessageInternal(resultMessage, false);

        return resultMessage;
      }
      catch (XMSException ex)
      {
        throw new MessagingException(
          ex,
          $"Error receiving message from queue {_configuration.Id}. See inner exception");
      }
    }

    private BaseMessage SendInternal(BaseMessage message)
    {
      try
      {
        _logger.LogDebug("Try to send message to queue with id: {queueId}", _configuration.Id);

        var mes = message.ConvertToMqMessage(GetSession());

        EnsureProducer();

        var deliveryMode = mes.JMSDeliveryMode == DeliveryMode.DELIVERY_MODE_NONE
          ? _producer.DeliveryMode
          : mes.JMSDeliveryMode;
        _producer.Send(mes, deliveryMode, mes.JMSPriority, mes.JMSExpiration);

        message.MessageId = mes.JMSMessageID;
        message.CorrelationId = mes.JMSCorrelationID;
        message.ApplicationTitle = !string.IsNullOrWhiteSpace(message.ApplicationTitle)
          ? message.ApplicationTitle
          : string.Empty;

        LogMessageInternal(message, true);

        return message;
      }
      catch (XMSException ex)
      {
        throw new MessagingException(
          ex,
          $"Error sending message to queue {_configuration.Id}. See inner exception");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "MQ PUT failure on queue with ID: {queueId}", _configuration.Id);
        throw new MessagingException(
          ex,
          $"Error sending message to queue {_configuration.Id}. See inner exception");
      }
    }

    [MemberNotNull(nameof(_destination))]
    private void EnsureDestination()
    {
      if (_destination == null)
      {
        if (_configuration.UseQueueString)
        {
          if (_configuration.QueueString.StartsWith("topic", StringComparison.InvariantCultureIgnoreCase))
          {
            _destination = _session.CreateTopic(_configuration.QueueString);
          }
          else
          {
            _destination = _session.CreateQueue(_configuration.QueueString);
          }
        }
        else
        {
          _destination = _session.CreateQueue($"queue:///{_configuration.QueueName}");
          _destination.SetIntProperty(XMSC.DELIVERY_MODE, XMSC.DELIVERY_PERSISTENT);
          _destination.SetIntProperty(XMSC.WMQ_FAIL_IF_QUIESCE, XMSC.WMQ_FIQ_YES);
        }
      }
    }

    private string CreateSelector(string correlationId, IEnumerable<(string Name, string Value)> additionalParameters)
    {
      var correlationIdSelector = string.Empty;
      if (!string.IsNullOrWhiteSpace(correlationId))
      {
        correlationIdSelector = $"(JMSCorrelationID=\'{correlationId}\')";
      }

      var combinedSelector = AddSelectorFromConfig(correlationIdSelector, additionalParameters);
      return combinedSelector;
    }

    private string AddSelectorFromConfig(string correlationIdSelector,
      IEnumerable<(string Name, string Value)> additionalParameters)
    {
      if (string.IsNullOrWhiteSpace(_configuration.Selector))
      {
        return correlationIdSelector;
      }

      var additionalSelector = additionalParameters.Aggregate(_configuration.Selector,
        (current, param) => current.Replace($":{param.Name}", $"\'{param.Value}\'"));

      var result = string.IsNullOrWhiteSpace(correlationIdSelector)
        ? additionalSelector
        : string.Join(" AND ", correlationIdSelector, additionalSelector);

      return result;
    }

    private IMessageConsumer CreateConsumer(string correlationId, (string Name, string Value)[] additionalParameters)
    {
      var session = GetSession();
      EnsureDestination();

      var selector = CreateSelector(correlationId, additionalParameters);

      return string.IsNullOrEmpty(selector)
        ? session.CreateConsumer(_destination)
        : session.CreateConsumer(_destination, selector);
    }

    private void LogMessageInternal(BaseMessage? message, bool isSend)
    {
      var prefix = isSend ? "sent to" : "received from";

      _logger.LogDebug(
        $"Message has been {prefix} queue with ID:{{queueId}}{Environment.NewLine}{{message}}",
        _configuration.Id,
        message?.LogBody());
    }

    [MemberNotNull(nameof(_producer))]
    private void EnsureProducer()
    {
      if (_producer == null)
      {
        lock (_producerLock)
        {
          if (_producer == null)
          {
            var session = GetSession();
            EnsureDestination();

            _producer = session.CreateProducer(_destination);

            GetConnection().Start();
          }
        }
      }
    }
  }
}