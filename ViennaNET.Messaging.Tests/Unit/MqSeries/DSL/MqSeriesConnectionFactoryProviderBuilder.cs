using System;
using IBM.XMS;
using Moq;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries.DSL
{
  class MqSeriesConnectionFactoryProviderBuilder
  {
    private Action<IMessage> _sender;
    private Func<long, IMessage> _producer;

    public Mock<IConnection> ConnectionMock { get; private set; }
    public Mock<ISession> SessionMock { get; private set; }
    public Mock<IMessageConsumer> MessageConsumerMock { get; private set; }

    public MqSeriesConnectionFactoryProviderBuilder WithMessageSender(Action<IMessage> sender)
    {
      _sender = sender;
      return this;
    }
    
    public MqSeriesConnectionFactoryProviderBuilder WithMessageProducer(Func<long, IMessage> producer)
    {
      _producer = producer;
      return this;
    }

    public IMqSeriesQueueConnectionFactoryProvider Please()
    {
      var producer = new Mock<IMessageProducer>();
      if (_sender != null)
      {
        producer
          .Setup(x => x.Send(It.IsAny<IMessage>()))
          .Callback<IMessage>(_sender);
      }

      var consumer = new Mock<IMessageConsumer>();
      if (_producer != null)
      {
        consumer
          .Setup(x => x.Receive())
          .Returns(() => _producer(TimeoutHelper.InfiniteWaitTimeout));
        consumer
          .Setup(x => x.Receive(It.IsAny<long>()))
          .Returns<long>(t => _producer(t));
        consumer
          .Setup(x => x.ReceiveNoWait())
          .Returns(() => _producer(TimeoutHelper.NoWaitTimeout));
      }
      MessageConsumerMock = consumer;

      var session = new Mock<ISession>();
      session
        .Setup(x => x.CreateQueue(It.IsAny<string>()))
        .Returns(new Mock<IDestination>().Object);
      session
        .Setup(x => x.CreateConsumer(It.IsAny<IDestination>()))
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
        .Setup(x => x.CreateSession(It.IsAny<bool>(), It.IsAny<AcknowledgeMode>()))
        .Returns(session.Object);
      ConnectionMock = connection;

      var connectionFactory = new Mock<IConnectionFactory>();
      connectionFactory
        .Setup(x => x.CreateConnection())
        .Returns(connection.Object);

      var connectionFactoryProvider = new Mock<IMqSeriesQueueConnectionFactoryProvider>();
      connectionFactoryProvider
        .Setup(x => x.GetConnectionFactory(It.IsAny<int>()))
        .Returns(connectionFactory.Object);

      return connectionFactoryProvider.Object;
    }
  }
}
