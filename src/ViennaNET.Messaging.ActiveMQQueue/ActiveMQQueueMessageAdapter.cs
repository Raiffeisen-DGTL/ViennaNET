using System.Diagnostics.CodeAnalysis;
using Apache.NMS;
using Apache.NMS.Util;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.ActiveMQQueue
{
    /// <summary>
    ///   Адаптер очереди ActiveMQ
    /// </summary>
    public class ActiveMqQueueMessageAdapter : IMessageAdapter
    {
        private static readonly object connectionLock = new();

        private readonly ActiveMqQueueConfiguration _configuration;

        private readonly IActiveMqConnectionFactory _connectionFactory;
        private readonly object _consumerLock = new();
        private readonly ILogger _logger;
        private readonly object _producerLock = new();
        private readonly object _sessionLock = new();
        private IConnection? _connection;
        private volatile IMessageConsumer? _consumer;
        private IDestination? _destination;
        private bool _isConnected;

        private bool _isDisposed;
        private volatile IMessageProducer? _producer;
        private volatile ISession? _session;

        /// <summary>
        ///   Конструктор адаптера
        /// </summary>
        /// <param name="connectionFactory">Фабрика подключений</param>
        /// <param name="queueConfiguration">Параметры очереди</param>
        /// <param name="logger">Сервис логирования</param>
        public ActiveMqQueueMessageAdapter(IActiveMqConnectionFactory connectionFactory,
          ActiveMqQueueConfiguration queueConfiguration,
          ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _configuration = queueConfiguration;
            _logger = logger;
        }

        /// <summary>
        ///   Возвращает (и при необходимости создаёт) <see cref="ISession" />
        /// </summary>
        protected ISession Session
        {
            get
            {
                if (_session == null)
                {
                    lock (_sessionLock)
                    {
                        if (_session == null)
                        {
                            ThrowIfNotConnected();
                            _session = CreateSession(_connection);
                        }
                    }
                }

                return _session;
            }
        }

        /// <summary>
        ///   Возвращает <see cref="IConnection" />
        /// </summary>
        protected IConnection Connection
        {
            get
            {
                ThrowIfNotConnected();
                return _connection;
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
                    var connectionFactory = _connectionFactory.GetConnectionFactory(_configuration);

                    _connection = string.IsNullOrWhiteSpace(_configuration.User)
                      ? connectionFactory.CreateConnection()
                      : connectionFactory.CreateConnection(_configuration.User, _configuration.Password);

                    if (!string.IsNullOrEmpty(_configuration.ClientId))
                    {
                        _connection.ClientId = _configuration.ClientId;
                    }

                    _logger.LogDebug("Established connection to queue with id: {queueId}", _configuration.Id);
                }
                catch (NMSException ex)
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
            catch (NMSException ex)
            {
                throw new MessagingException(
                  ex,
                  $"Error disconnecting from the queue {_configuration.Id}. See inner exception");
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public BaseMessage Receive(string correlationId = null, TimeSpan? timeout = null,
          params (string Name, string Value)[] additionalParameters)
        {
            ThrowIfDisposed();
            ThrowIfNotConnected();

            var msg = ReceiveInternal(correlationId, timeout, additionalParameters);
            return msg ?? throw new MessageDidNotReceivedException(
              $"Can not receive message because queue {_configuration.Id} is empty");
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
        ~ActiveMqQueueMessageAdapter()
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
        ///   Создаёт <see cref="ISession" /> с подходящими настройками
        /// </summary>
        /// <returns>
        ///   <see cref="ISession" />
        /// </returns>
        protected virtual ISession CreateSession(IConnection connection)
        {
            return connection.CreateSession();
        }

        /// <summary>
        ///   Возвращает (и при необходимости создаёт) <see cref="IMessageConsumer" />
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="additionalParameters"></param>
        /// <returns>
        ///   <see cref="IMessageConsumer" />
        /// </returns>
        protected IMessageConsumer GetConsumer(string? correlationId = null,
          params (string Name, string Value)[] additionalParameters)
        {
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                var specialConsumer = CreateConsumer(correlationId, additionalParameters);
                Connection.Start();
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
                Connection.Start();
            }

            return _consumer;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(ActiveMqQueueMessageAdapter));
            }
        }

        [MemberNotNull(nameof(_connection))]
        private void ThrowIfNotConnected()
        {
            if (!_isConnected || _connection == null)
            {
                throw new MessagingException($"Connection to queue {_configuration.Id} is not open");
            }
        }

        /// <summary>
        ///   Отключение всех элементов
        /// </summary>
        protected void DisconnectInternal()
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

            if (_destination != null)
            {
                _destination.Dispose();
                _destination = null;
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

        private BaseMessage? ReceiveInternal(string? correlationId, TimeSpan? timeout,
          params (string Name, string Value)[] additionalParameters)
        {
            try
            {
                _logger.LogDebug("Try to receive message from queue with id: {queueId}", _configuration.Id);

                var consumer = GetConsumer(correlationId, additionalParameters);

                IMessage receivedMessage;
                var waitTimeout = TimeoutHelper.GetTimeout(timeout);
                if (waitTimeout > 0L)
                {
                    receivedMessage = consumer.Receive(TimeSpan.FromMilliseconds(waitTimeout));
                }
                else if (waitTimeout == TimeoutHelper.NoWaitTimeout)
                {
                    receivedMessage = consumer.ReceiveNoWait();
                }
                else
                {
                    receivedMessage = consumer.Receive();
                }

                if (receivedMessage == null)
                {
                    return null;
                }

                var resultMessage = receivedMessage?.ConvertToBaseMessage();

                LogMessageInternal(resultMessage, false);

                return resultMessage;
            }
            catch (NMSException nmsex)
            {
                throw new MessagingException(
                  nmsex,
                  $"Error receiving message from queue {_configuration.Id}. See inner exception");
            }
        }

        private BaseMessage SendInternal(BaseMessage message)
        {
            try
            {
                _logger.LogDebug("Try to send message to queue with id: {queueId}", _configuration.Id);

                var mes = message.ConvertToMqMessage(Session);

                EnsureProducer();

                var deliveryMode = mes.NMSDeliveryMode == MsgDeliveryMode.NonPersistent
                  ? _producer.DeliveryMode
                  : mes.NMSDeliveryMode;

                _producer.Send(mes, deliveryMode, mes.NMSPriority, mes.NMSTimeToLive);

                message.MessageId = mes.NMSMessageId;
                message.CorrelationId = mes.NMSCorrelationID;
                message.ApplicationTitle = !string.IsNullOrWhiteSpace(message.ApplicationTitle)
                  ? message.ApplicationTitle
                  : string.Empty;

                LogMessageInternal(message, true);

                return message;
            }
            catch (NMSException ex)
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

        private string CreateSelector(string? correlationId, IEnumerable<(string Name, string Value)> additionalParameters)
        {
            var correlationIdSelector = string.Empty;
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                correlationIdSelector = $"(JMSCorrelationID=\'{correlationId}\')";
            }

            var combinedSelector = AddSelectorFromConfig(correlationIdSelector, additionalParameters);
            return combinedSelector;
        }

        private void EnsureDestination()
        {
            if (_destination == null)
            {
                if (_configuration.UseQueueString)
                {
                    _destination = SessionUtil.GetDestination(_session, _configuration.QueueString);
                }
                else
                {
                    _destination = _session.GetQueue(_configuration.QueueName);
                }
            }
        }

        private string AddSelectorFromConfig(string correlationIdSelector,
          IEnumerable<(string Name, string Value)> additionalParameters)
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

            return result;
        }

        private IMessageConsumer CreateConsumer(string? correlationId, (string Name, string Value)[] additionalParameters)
        {
            var session = Session;
            EnsureDestination();

            var selector = CreateSelector(correlationId, additionalParameters);

            return string.IsNullOrEmpty(selector)
              ? session.CreateConsumer(_destination)
              : session.CreateConsumer(_destination, selector);
        }

        /// <summary>
        ///   Логгирование сообщения
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="isSend">true - как отправку, false - как получение</param>
        protected void LogMessageInternal(BaseMessage message, bool isSend)
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
                        var session = Session;
                        EnsureDestination();

                        _producer = session.CreateProducer(_destination);

                        Connection.Start();
                    }
                }
            }
        }
    }
}