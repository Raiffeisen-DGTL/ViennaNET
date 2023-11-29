using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apache.NMS;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.ActiveMQQueue
{
  /// <summary>
  ///   Адаптер, реализующий взаимодействие с очередью ActiveMQ в режиме подписки
  /// </summary>
  /// <remarks>Не поддерживает транзакции</remarks>
  public class ActiveMqQueueSubscribingMessageAdapter : ActiveMqQueueMessageAdapter, IMessageAdapterWithSubscribing
  {
    private const int DefaultReplyTimeout = 30;

    private readonly ActiveMqQueueConfiguration _configuration;
    private readonly List<MessageListener> _listeners = new();

    /// <summary>
    ///   Конструктор адаптера
    /// </summary>
    /// <param name="connectionFactory">Фабрика подключений</param>
    /// <param name="queueConfiguration">Параметры очереди</param>
    /// <param name="logger">Сервис логирования</param>
    public ActiveMqQueueSubscribingMessageAdapter(IActiveMqConnectionFactory connectionFactory,
      ActiveMqQueueConfiguration queueConfiguration,
      ILogger logger)
      : base(connectionFactory, queueConfiguration, logger)
    {
      _configuration = queueConfiguration;
    }

    /// <inheritdoc />
    public void Subscribe(Func<BaseMessage, Task> handler)
    {
      handler.ThrowIfNull(nameof(handler));

      var listener = new MessageListener(msg => handler.Invoke(msg.ConvertToBaseMessage()));
      _listeners.Add(listener);

      GetConsumer().Listener += listener;
    }

    /// <inheritdoc />
    public void Unsubscribe()
    {
      var consumer = GetConsumer();

      foreach (var listener in _listeners)
      {
        consumer.Listener -= listener;
      }

      _listeners.Clear();
    }

    /// <inheritdoc />
    public async Task<BaseMessage> RequestAndWaitResponse(BaseMessage message)
    {
      var replyQueueName = _configuration.ReplyQueue;
      message.ReplyQueue = replyQueueName;
      message.LifeTime = _configuration.Lifetime ?? TimeSpan.FromSeconds(DefaultReplyTimeout);

      Send(message);

      var destination = Session.GetQueue(_configuration.ReplyQueue);
      var consumer = Session.CreateConsumer(
        destination,
        $"JMSCorrelationID = '{message.CorrelationId}'",
        false);

      var result = await Task
        .Factory
        .StartNew(() => consumer.Receive(message.LifeTime))
        .ConfigureAwait(false);

      if (result == null)
      {
        throw new TimeoutException($"Request timed out. ReplyQueueName: {replyQueueName}");
      }

      var resultMessage = result.ConvertToBaseMessage();

      LogMessageInternal(resultMessage, false);

      return resultMessage;
    }

    /// <inheritdoc />
    public BaseMessage Reply(BaseMessage message)
    {
      var replyQueueName = _configuration.ReplyQueue;

      message.ReplyQueue = replyQueueName;
      message.LifeTime = _configuration.Lifetime ?? TimeSpan.FromSeconds(DefaultReplyTimeout);

      var destination = Session.GetQueue(_configuration.ReplyQueue);
      using var producer = Session.CreateProducer(destination);

      LogMessageInternal(message, true);

      var requestMessage = message.ConvertToMqMessage(Session);

      producer.Send(requestMessage);

      message.MessageId = requestMessage.NMSMessageId;
      message.CorrelationId = requestMessage.NMSCorrelationID;

      return message;
    }

    /// <inheritdoc />
    protected override ISession CreateSession(IConnection connection)
    {
      return connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
    }
  }
}