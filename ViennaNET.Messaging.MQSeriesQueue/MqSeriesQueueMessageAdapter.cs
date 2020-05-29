using System;
using System.Linq;
using System.Text;
using IBM.XMS;
using ViennaNET.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  /// <inheritdoc />
  public class MqSeriesQueueMessageAdapter : IMessageAdapterWithTransactions
  {
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
          _connectionFactory.SetIntProperty(XMSC.WMQ_BROKER_VERSION, XMSC.WMQ_BROKER_DEFAULT); //TODO is it necessary?

          _connection = string.IsNullOrEmpty(_configuration.User)
            ? _connectionFactory.CreateConnection()
            : _connectionFactory.CreateConnection(_configuration.User, _configuration.Password);

          _connection.ClientID = _configuration.ClientId;
        }
        catch (XMSException ex)
        {
          DisconectInternal();
          Logger.LogDebug("MQ connect failure:");
          LogXmsException(ex);
          throw new MessagingException(ex, "Error while connect to the queue. See inner exception for more details");
        }
        catch (Exception ex)
        {
          Logger.LogError(ex, "Error while connect to the queue. See inner exception for more details");
          DisconectInternal();
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
        DisconectInternal();
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

    /// <summary>
    ///   Получить сообщение из очереди
    /// </summary>
    /// <param name="correlationId">Correlation id of message</param>
    /// <returns>Полученное сообщение</returns>
    public BaseMessage Receive(string correlationId = null)
    {
      ThrowIfDisposed();
      ThrowIfNotConnected();

      var msg = ReceiveInternal(false, correlationId);
      if (msg == null)
      {
        throw new MessagingException("Can not receive message because queue is empty");
      }

      return msg;
    }

    /// <summary>
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="correlationId"></param>
    /// <returns>True если сообщение было считано из очереди. False если сообщение не было считано из очереди</returns>
    public bool TryReceive(out BaseMessage message, string correlationId = null)
    {
      ThrowIfDisposed();
      ThrowIfNotConnected();

      var msg = ReceiveInternal(false, correlationId);
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
      return processingType == MessageProcessingType.ThreadStrategy;
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
        var mes = ConvertToInternalMessage(message);
        mes.JMSMessageID = string.IsNullOrWhiteSpace(message.MessageId)
          ? Guid.NewGuid()
                .ToString()
                .ToUpper()
          : message.MessageId;
        mes.JMSCorrelationID = string.IsNullOrWhiteSpace(message.CorrelationId)
          ? mes.JMSMessageID
          : message.CorrelationId;
        mes.JMSExpiration = (int)message.LifeTime.TotalSeconds * 10; // взято из MQSeriesMessageAdapter

        if (!string.IsNullOrWhiteSpace(message.ReplyQueue))
        {
          var replayQueue = GetSession()
            .CreateQueue(message.ReplyQueue.Trim());
          mes.JMSReplyTo = replayQueue;
        }

        foreach (var kv in message.Properties)
        {
          mes.SetObjectProperty(kv.Key, kv.Value);
        }

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

    private IMessage ConvertToInternalMessage(BaseMessage message)
    {
      switch (message)
      {
        case TextMessage textMessage:
          return GetSession()
            .CreateTextMessage(textMessage.Body);
        case BytesMessage bytesMessage:
          var result = GetSession()
            .CreateBytesMessage();
          result.WriteBytes(bytesMessage.Body);
          return result;
        default: throw new ArgumentException($"Unknown inherited type of BaseMessage ({message.GetType()}) while converting to IMessage");
      }
    }

    private static BaseMessage ConvertToBaseMessage(IMessage message)
    {
      switch (message)
      {
        case ITextMessage textMessage:
          return new TextMessage { Body = textMessage.Text };
        case IBytesMessage bytesMessage:
          var buffer = new byte[bytesMessage.BodyLength];
          bytesMessage.ReadBytes(buffer);
          return new BytesMessage { Body = buffer };
        default:
          return new TextMessage { Body = string.Empty };
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
          FreeResources();
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

    ~MqSeriesQueueMessageAdapter()
    {
      try
      {
        FreeResources();
      }
      catch
      {
        // ignored
      }
    }

    private BaseMessage ReceiveInternal(bool isBrowse, string correlationId = null)
    {
      try
      {
        IMessage receivedMessage;
        if (isBrowse)
        {
          var browser = GetBrowser(correlationId);
          receivedMessage = (IMessage)browser.GetEnumerator()
                                             .Current;
        }
        else
        {
          receivedMessage = GetConsumer(correlationId)
            .ReceiveNoWait();
        }

        if (receivedMessage == null)
        {
          return null;
        }

        var sendDate = ConvertJavaTimestampToDateTime(receivedMessage.JMSTimestamp);
        var expirationDate = ConvertJavaTimestampToDateTime(receivedMessage.JMSExpiration);

        var message = ConvertToBaseMessage(receivedMessage);
        message.MessageId = receivedMessage.JMSMessageID;
        message.CorrelationId = receivedMessage.JMSCorrelationID;
        message.SendDateTime = sendDate;
        message.ReceiveDate = DateTime.Now;
        message.ReplyQueue = receivedMessage.JMSReplyTo?.Name;
        message.LifeTime = expirationDate > sendDate
          ? expirationDate - sendDate
          : new TimeSpan();

        // чтение properties сообщения web sphere.
        var propertyNames = receivedMessage.PropertyNames;
        var messageProps = string.Empty;
        while (propertyNames.MoveNext())
        {
          var name = (string)propertyNames.Current ?? string.Empty;
          var prop = receivedMessage.GetObjectProperty(name);
          message.Properties.Add(name, prop);
          messageProps += name + " = " + prop + ";\n";
        }

        Logger.LogDebugFormat("Message Properties: \n" + messageProps);

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

    private IMessageConsumer GetConsumer(string correlationId = "")
    {
      if (!string.IsNullOrWhiteSpace(correlationId))
      {
        var specialConsumer = CreateConsumer(correlationId);
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

        _consumer = CreateConsumer(correlationId);
        _connection.Start();
      }

      return _consumer;
    }

    private IMessageConsumer CreateConsumer(string corellationId)
    {
      var session = GetSession();
      var destination = GetDestination(session);

      var corellationIdSelector = "";
      if (!string.IsNullOrWhiteSpace(corellationId))
      {
        corellationIdSelector = $"JMSCorrelationID=\'{corellationId}\'";
      }

      var combinedSelector = string.Join(" AND ", new[] { string.Empty, corellationIdSelector }.Where(s => !string.IsNullOrWhiteSpace(s))
                                                                                               .Select(s => s));
      combinedSelector = AddSelectorFromConfig(combinedSelector);

      return string.IsNullOrEmpty(combinedSelector)
        ? session.CreateConsumer(destination)
        : session.CreateConsumer(destination, combinedSelector);
    }

    internal string AddSelectorFromConfig(string combinedSelector)
    {
      if (_configuration.Selectors != null)
      {
        var selectorsStr = _configuration.Selectors.Select(s => $"{s.Key} = \'{s.Value}\'");

        var customSelector = string.Join(" AND ", selectorsStr);

        if (!string.IsNullOrEmpty(combinedSelector))
        {
          combinedSelector = string.Join(" AND ", combinedSelector, customSelector);
        }
        else
        {
          combinedSelector = customSelector;
        }
      }

      Logger.LogInfo($"MQ Selector: {combinedSelector}");

      return combinedSelector;
    }

    private IQueueBrowser GetBrowser(string correlationId = "")
    {
      if (!string.IsNullOrWhiteSpace(correlationId))
      {
        var specialBrowser = CreateBrowser(correlationId);
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

        _browser = CreateBrowser(correlationId);
        _connection.Start();
      }

      return _browser;
    }

    private IQueueBrowser CreateBrowser(string corellationId)
    {
      var session = GetSession();
      var destination = GetDestination(session);

      var corellationIdSelector = "";
      if (!string.IsNullOrWhiteSpace(corellationId))
      {
        corellationIdSelector = $"JMSCorrelationID=\'{corellationId}\'";
      }

      var combinedSelector = string.Join(" AND ", new[] { string.Empty, corellationIdSelector }.Where(s => !string.IsNullOrWhiteSpace(s))
                                                                                               .Select(s => "(" + s + ")"));
      return string.IsNullOrEmpty(combinedSelector)
        ? session.CreateBrowser(destination)
        : session.CreateBrowser(destination, combinedSelector);
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

    private void FreeResources()
    {
      DisconectInternal();
      _isDisposed = true;
    }

    private void DisconectInternal()
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

    private static DateTime ConvertJavaTimestampToDateTime(long jmsTimestamp)
    {
      // Example:  Converting Java millis time to .NET time
      var baseTime = new DateTime(1970, 1, 1, 0, 0, 0);
      var utcTimeTicks = jmsTimestamp * 10000 + baseTime.Ticks;
      return new DateTime(utcTimeTicks, DateTimeKind.Utc);
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
  }
}