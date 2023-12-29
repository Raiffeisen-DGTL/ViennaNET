using Apache.NMS;
using Moq;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
    internal class ActiveMqConnectionFactoryBuilder
  {
    private Func<TimeSpan, IMessage> _producer;
    private Action<IMessage> _sender;

    public Mock<IConnection> ConnectionMock { get; private set; }
    public Mock<ISession> SessionMock { get; private set; }
    public Mock<IMessageConsumer> MessageConsumerMock { get; private set; }

    public ActiveMqConnectionFactoryBuilder WithMessageSender(Action<IMessage> sender)
    {
      _sender = sender;
      return this;
    }

    public ActiveMqConnectionFactoryBuilder WithMessageProducer(Func<TimeSpan, IMessage> producer)
    {
      _producer = producer;
      return this;
    }

    public IActiveMqConnectionFactory Please()
    {
      var producer = new Mock<IMessageProducer>();
      if (_sender != null)
      {
        producer
          .Setup(x => x.Send(It.IsAny<IMessage>()))
          .Callback(_sender);
        producer
          .Setup(x => x.Send(It.IsAny<IMessage>(), It.IsAny<MsgDeliveryMode>(), It.IsAny<MsgPriority>(),
            It.IsAny<TimeSpan>()))
          .Callback<IMessage, MsgDeliveryMode, MsgPriority, TimeSpan>(
            (msg, deliveryMode, priority, timeToLive) =>
            {
              _sender(msg);
              msg.NMSDeliveryMode = deliveryMode;
              msg.NMSPriority = priority;
              msg.NMSTimestamp = msg.NMSTimestamp + timeToLive;
            });
      }

      var consumer = new Mock<IMessageConsumer>();
      if (_producer != null)
      {
        consumer
          .SetupAdd(x => x.Listener += mes => { });
        consumer
          .SetupRemove(x => x.Listener -= null);
        consumer
          .Setup(x => x.Receive())
          .Returns(() => _producer(TimeSpan.MaxValue));
        consumer
          .Setup(x => x.Receive(It.IsAny<TimeSpan>()))
          .Returns<TimeSpan>(t => _producer(t));
        consumer
          .Setup(x => x.ReceiveNoWait())
          .Returns(() => _producer(TimeSpan.Zero));
      }

      MessageConsumerMock = consumer;

      var session = new Mock<ISession>();
      session
        .Setup(x => x.GetQueue(It.IsAny<string>()))
        .Returns(new Mock<IQueue>().Object);
      session
        .Setup(x => x.CreateConsumer(It.IsAny<IDestination>()))
        .Returns(consumer.Object);
      session
        .Setup(x => x.CreateConsumer(It.IsAny<IDestination>(), It.IsAny<string>()))
        .Returns(consumer.Object);
      session
        .Setup(x => x.CreateProducer(It.IsAny<IDestination>()))
        .Returns(producer.Object);
      session
        .Setup(x => x.CreateTextMessage(It.IsAny<string>()))
        .Returns<string>(s => Mock.Of<ITextMessage>(x => x.Text == s));
      SessionMock = session;

      var connection = new Mock<IConnection>();
      connection
        .Setup(x => x.CreateSession())
        .Returns(session.Object);
      connection
        .Setup(x => x.CreateSession(It.IsAny<AcknowledgementMode>()))
        .Returns(session.Object);
      ConnectionMock = connection;

      var connectionFactory = new Mock<IConnectionFactory>();
      connectionFactory
        .Setup(x => x.CreateConnection())
        .Returns(connection.Object);

      var connectionFactoryProvider = new Mock<IActiveMqConnectionFactory>();
      connectionFactoryProvider
        .Setup(x => x.GetConnectionFactory(It.IsAny<ActiveMqQueueConfiguration>()))
        .Returns(connectionFactory.Object);

      return connectionFactoryProvider.Object;
    }
  }
}