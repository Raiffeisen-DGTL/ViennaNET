using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IBM.XMS;
using ViennaNET.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  /// <summary>
  ///   Адаптер, реализующий взаимодействие с очередью IBM MQ
  /// </summary>
  public class MqSeriesQueueMessageAdapter : IMessageAdapterWithTransactions, IMessageAdapterWithSubscribing
  {
    private const long NoWaitTimeout = -1L;
    private const long InfiniteWaitTimeout = 0L;

    private static readonly object ConnectionLock = new object();
    private readonly object _browserLock = new object();
    private readonly MqSeriesQueueConfiguration _configuration;
    private readonly object _consumerLock = new object();
    private readonly object _producerLock = new object();

    private volatile IQueueBrowser _browser;
    private IConnection _connection;
    private IConnectionFactory _connectionFactory;
    private volatile IMessageConsumer _consumer;
    private IDestination _destination;
    private bool _isConnected;
    private bool _isDisposed;
    private volatile IMessageProducer _producer;
    private ISession _session;

    /// <summary>
    ///   Инициализирует экземпляр переменной типа <see cref="MqSeriesQueueConfiguration" />
    /// </summary>
    /// <param name="configuration"></param>
    public MqSeriesQueueMessageAdapter(MqSeriesQueueConfiguration configuration)
    {
      _configuration = configuration.ThrowIfNull(nameof(configuration));
    }

    /// <inheritdoc />
    public QueueConfigurationBase Configuration => _configuration;

    /// <summary>
    ///   Подключение к очереди
    /// </summary>
    public void Connect()
    {
      ThrowIfDisposed();
      if (_isConnected)
      {
        return;
      }

      lock (ConnectionLock)
      {
        try
        {
          var factoryFactory = XMSFactoryFactory.GetInstance(XMSC.CT_WMQ);
          _connectionFactory = factoryFactory.CreateConnectionFactory();
          _connectionFactory.SetStringProperty(XMSC.WMQ_HOST_NAME, _configuration.Server);
          _connectionFactory.SetIntProperty(XMSC.WMQ_PORT, _configuration.Port);
          _connectionFactory.SetStringProperty(XMSC.WMQ_CHANNEL, _configuration.Channel);
          _connectionFactory.SetIntProperty(XMSC.WMQ_CONNECTION_MODE, XMSC.WMQ_CM_CLIENT);
          _connectionFactory.SetStringProperty(XMSC.WMQ_QUEUE_MANAGER, _configuration.QueueManager);
          _connectionFactory.SetIntProperty(XMSC.WMQ_BROKER_VERSION, XMSC.WMQ_BROKER_DEFAULT);

          _connection = string.IsNullOrEmpty(_configuration.User)
            ? _connectionFactory.CreateConnection()
            : _connectionFactory.CreateConnection(_configuration.User, _configuration.Password);

          _connection.ClientID = _configuration.ClientId;
        }
        catch (XMSException ex)
        {
          DisconnectInternal();
          Logger.LogDebug("MQ connect failure:");
          LogXmsException(ex);
          throw new MessagingException(ex, "Error while connect to the queue. See inner exception for more details");
        }
        catch (Exception ex)
        {
          Logger.LogError(ex, "Error while connect to the queue. See inner exception for more details");
          DisconnectInternal();
          throw;
        }
      }

      _isConnected = true;
    }

    /// <summary>
    ///   Отключения от очереди
    /// </summary>
    public void Disconnect()
    {
      ThrowIfDisposed();
      try
      {
        DisconnectInternal();
      }
      catch (XMSException ex)
      {
        Logger.LogDebug("MQ disconnect failure:");
        LogXmsException(ex);
        throw new MessagingException(ex, "Error while disconnect from the queue. See inner exception for more details");
      }
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

    /// <summary>
    ///   Отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение для отправки</param>
    /// <returns>Отправить сообщение с дополнительной информацией</returns>
    public BaseMessage Send(BaseMessage message)
    {
      message.ThrowIfNull(nameof(message));

      ThrowIfDisposed();
      ThrowIfNotConnected();

      return SendInternal(message);
    }

    /// <inheritdoc />
    public BaseMessage Receive(string correlationId = null, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      ThrowIfDisposed();
      ThrowIfNotConnected();

      var msg = ReceiveInternal(false, correlationId, timeout, additionalParameters);
      if (msg == null)
      {
        throw new MessageDidNotReceivedException("Can not receive message because queue is empty");
      }

      return msg;
    }

    /// <inheritdoc />
    public bool TryReceive(
      out BaseMessage message, string correlationId = null, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters)
    {
      ThrowIfDisposed();
      ThrowIfNotConnected();

      var msg = ReceiveInternal(false, correlationId, timeout, additionalParameters);
      if (msg != null)
      {
        message = msg;
        return true;
      }

      message = null;
      return false;
    }

    /// <summary>
    ///   Public implementation of Dispose pattern callable by consumers
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Выполнить коммит, если сессия в транзакционном режиме
    /// </summary>
    public void CommitIfTransacted(BaseMessage message = null)
    {
      try
      {
        if (_configuration.TransactionEnabled)
        {
          _session.Commit();
        }
      }
      catch (XMSException ex)
      {
        Logger.LogDebug("MQ session commit failure:");
        LogXmsException(ex);
        throw new MessagingException(ex, "Error while commit message. See inner exception for more details");
      }
    }

    /// <summary>
    ///   Выполнить откат, если сессия в транзакционном режиме
    /// </summary>
    public void RollbackIfTransacted()
    {
      try
      {
        if (_configuration.TransactionEnabled)
        {
          _session.Rollback();
        }
      }
      catch (XMSException ex)
      {
        Logger.LogDebug("MQ session rollback failure:");
        LogXmsException(ex);
        throw new MessagingException(ex, "Error while rollback message. See inner exception for more details");
      }
    }

    /// <inheritdoc />
    public bool SupportProcessingType(MessageProcessingType processingType)
    {
      return processingType == MessageProcessingType.ThreadStrategy || processingType == MessageProcessingType.Subscribe;
    }

    private static void LogXmsException(XMSException ex)
    {
      Logger.LogDebugFormat("ex.ErrorCode - {0}", ex.ErrorCode);
      Logger.LogDebugFormat("ex.LinkedException.Message - {0}", ex.LinkedException.Return(x => x.Message));
      Logger.LogDebugFormat("ex.LinkedException.InnerException - {0}", ex.LinkedException.Return(x => x.InnerException));
      Logger.LogDebugFormat("ex.Message - {0}", ex.Message);
      Logger.LogDebug(ex.ToString());
      Logger.LogDebug("-----------------------------------------------------------------");
    }

    private BaseMessage SendInternal(BaseMessage message)
    {
      try
      {
        var mes = message.ConvertToMqMessage(GetSession());

        GetProducer()
          .Send(mes);

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
        Logger.LogDebug("MQ PUT failure:");
        LogXmsException(ex);

        throw new MessagingException(ex, "Messaging error while sending message. See inner exception for more details");
      }
      catch (Exception ex)
      {
        throw new MessagingException(ex, "Error while sending message. See inner exception for more details");
      }
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
          Logger.LogErrorFormat(ex.ToString());
        }
      }

      // Free any unmanaged objects here.
      //
      _isDisposed = true;
    }

    /// <inheritdoc />
    ~MqSeriesQueueMessageAdapter()
    {
      Dispose(false);
    }

    private BaseMessage ReceiveInternal(
      bool isBrowse, string correlationId, TimeSpan? timeout, params (string Name, string Value)[] additionalParameters)
    {
      try
      {
        IMessage receivedMessage;
        if (isBrowse)
        {
          var browser = GetBrowser(correlationId, additionalParameters);
          receivedMessage = (IMessage)browser.GetEnumerator()
                                             .Current;
        }
        else
        {
          var waitTimeout = GetTimeout(timeout);
          if (waitTimeout > 0L)
          {
            receivedMessage = GetConsumer(correlationId, additionalParameters)
              .Receive(waitTimeout);
          }
          else if (waitTimeout == NoWaitTimeout)
          {
            receivedMessage = GetConsumer(correlationId, additionalParameters)
              .ReceiveNoWait();
          }
          else
          {
            receivedMessage = GetConsumer(correlationId, additionalParameters)
              .Receive();
          }
        }

        if (receivedMessage == null)
        {
          return null;
        }

        var message = receivedMessage.ConvertToBaseMessage();

        Logger.LogDebugFormat("Received message: {0}", receivedMessage);
        LogMessageInternal(message, false);

        return message;
      }
      catch (XMSException ex)
      {
        Logger.LogDebug("MQ GET failure:");
        LogXmsException(ex);
        throw new MessagingException(ex, "Error while receiving message. See inner exception for more details");
      }
    }

    internal static long GetTimeout(TimeSpan? timeout)
    {
      if (timeout == Timeout.InfiniteTimeSpan || timeout == TimeSpan.MaxValue)
      {
        return InfiniteWaitTimeout;
      }

      if (timeout == TimeSpan.MinValue)
      {
        return NoWaitTimeout;
      }

      var waitTimeout = timeout.HasValue
        ? (long)timeout.Value.TotalMilliseconds
        : NoWaitTimeout;

      return waitTimeout <= 0L
        ? NoWaitTimeout
        : waitTimeout;
    }

    private IMessageProducer GetProducer()
    {
      if (_producer != null)
      {
        return _producer;
      }

      lock (_producerLock)
      {
        if (_producer != null)
        {
          return _producer;
        }

        var session = GetSession();
        var destination = GetDestination(session);

        _producer = session.CreateProducer(destination);

        _connection.Start();
      }

      return _producer;
    }

    private IMessageConsumer GetConsumer(string correlationId = "", params (string Name, string Value)[] additionalParameters)
    {
      if (!string.IsNullOrWhiteSpace(correlationId))
      {
        var specialConsumer = CreateConsumer(correlationId, additionalParameters);
        _connection.Start();
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
        _connection.Start();
      }

      return _consumer;
    }

    private IMessageConsumer CreateConsumer(string correlationId, (string Name, string Value)[] additionalParameters)
    {
      var session = GetSession();
      var destination = GetDestination(session);

      var selector = CreateSelector(correlationId, additionalParameters);

      return string.IsNullOrEmpty(selector)
        ? session.CreateConsumer(destination)
        : session.CreateConsumer(destination, selector);
    }

    internal string CreateSelector(string correlationId, IEnumerable<(string Name, string Value)> additionalParameters)
    {
      var correlationIdSelector = string.Empty;
      if (!string.IsNullOrWhiteSpace(correlationId))
      {
        correlationIdSelector = $"(JMSCorrelationID=\'{correlationId}\')";
      }

      var combinedSelector = AddSelectorFromConfig(correlationIdSelector, additionalParameters);
      return combinedSelector;
    }

    private string AddSelectorFromConfig(string correlationIdSelector, IEnumerable<(string Name, string Value)> additionalParameters)
    {
      if (string.IsNullOrWhiteSpace(_configuration.Selector))
      {
        return correlationIdSelector;
      }

      var additionalSelector = additionalParameters.Aggregate(_configuration.Selector,
                                                              (current, param) =>
                                                                current.Replace($":{param.Name}", $"\'{param.Value}\'"));

      var result = string.IsNullOrWhiteSpace(correlationIdSelector)
        ? additionalSelector
        : string.Join(" AND ", correlationIdSelector, additionalSelector);

      Logger.LogInfo($"MQ Selector: {result}");

      return result;
    }

    private IQueueBrowser GetBrowser(string correlationId = "", params (string Name, string Value)[] additionalParameters)
    {
      if (!string.IsNullOrWhiteSpace(correlationId))
      {
        var specialBrowser = CreateBrowser(correlationId, additionalParameters);
        _connection.Start();
        return specialBrowser;
      }

      if (_browser != null)
      {
        return _browser;
      }

      lock (_browserLock)
      {
        if (_browser != null)
        {
          return _browser;
        }

        _browser = CreateBrowser(correlationId, additionalParameters);
        _connection.Start();
      }

      return _browser;
    }

    private IQueueBrowser CreateBrowser(string correlationId, (string Name, string Value)[] additionalParameters)
    {
      var session = GetSession();
      var destination = GetDestination(session);

      var selector = CreateSelector(correlationId, additionalParameters);

      return string.IsNullOrEmpty(selector)
        ? session.CreateBrowser(destination)
        : session.CreateBrowser(destination, selector);
    }

    private IDestination GetDestination(ISession session)
    {
      if (_destination != null)
      {
        return _destination;
      }

      if (_configuration.QueueString.StartsWith("topic", StringComparison.InvariantCultureIgnoreCase))
      {
        _destination = session.CreateTopic(_configuration.QueueString);
      }
      else
      {
        _destination = session.CreateQueue(_configuration.QueueString);
      }

      if (!_configuration.UseQueueString)
      {
        _destination.SetIntProperty(XMSC.DELIVERY_MODE, XMSC.DELIVERY_PERSISTENT);
        _destination.SetIntProperty(XMSC.WMQ_FAIL_IF_QUIESCE, XMSC.WMQ_FIQ_YES);
      }

      return _destination;
    }

    private ISession GetSession()
    {
      if (_session != null)
      {
        return _session;
      }

      _session = _configuration.TransactionEnabled
        ? _connection.CreateSession(true, AcknowledgeMode.SessionTransacted)
        : _connection.CreateSession(false, AcknowledgeMode.AutoAcknowledge);
      return _session;
    }

    private void ThrowIfNotConnected()
    {
      if (!_isConnected)
      {
        throw new MessagingException("Connection is not open");
      }
    }

    private void ThrowIfDisposed()
    {
      if (_isDisposed)
      {
        throw new MessagingException("Adapter already disposed");
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

    private static void LogMessageInternal(BaseMessage msg, bool isSend)
    {
      Logger.LogDebug(new StringBuilder().AppendLine(isSend
                                                       ? "Message has been sent"
                                                       : "Message has been received")
                                         .AppendLine($"MessageId = {msg.MessageId}")
                                         .AppendLine($"CorrelationId = {msg.CorrelationId}")
                                         .AppendLine($"Body = {msg.LogBody()}")
                                         .ToString());
    }

    /// <inheritdoc />
    public void Subscribe(Func<BaseMessage, Task> handler)
    {
      ThrowIfDisposed();
      ThrowIfNotConnected();

      GetConsumer()
        .MessageListener = msg => handler.Invoke(msg.ConvertToBaseMessage());
    }

    /// <inheritdoc />
    public void Unsubscribe()
    {
      GetConsumer()
        .MessageListener = null;
    }

    /// <inheritdoc />
    public Task<BaseMessage> RequestAndWaitResponse(BaseMessage message)
    {
      // TODO Данный метод нужно решить на другом уровне абстракции
      return Task.FromResult<BaseMessage>(null);
    }

    /// <inheritdoc />
    public BaseMessage Reply(BaseMessage message)
    {
      // TODO Данный метод нужно решить на другом уровне абстракции
      return null;
    }
  }
}