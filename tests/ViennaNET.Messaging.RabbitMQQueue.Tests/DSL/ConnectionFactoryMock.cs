using System;
using System.Collections.Generic;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests.DSL
{
  internal class ConnectionFactoryMock : IRabbitMqConnectionFactory
  {
    private BasicGetResult _getResult;

    public Mock<IModel> ChannelMock = new();

    public Mock<IConnection> ConnectionMock = new();

    public IAsyncBasicConsumer Consumer;

    public IConnection Create(RabbitMqQueueConfiguration config)
    {
      ChannelMock
        .Setup(m => m.CreateBasicProperties())
        .Returns(() => Given.BasicProperties);
      ChannelMock
        .Setup(m => m.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(),
          It.IsAny<IDictionary<string, object>>()))
        .Returns<string, bool, bool, bool, IDictionary<string, object>>((queueName, _1, _2, _3, _) =>
          new QueueDeclareOk(queueName, 0, 1));
      ChannelMock
        .Setup(m => m.QueueDeclarePassive(It.IsAny<string>()))
        .Returns<string>(queueName => new QueueDeclareOk(queueName, 0, 1));
      ChannelMock
        .Setup(m => m.BasicConsume(It.IsAny<string>(), It.IsAny<bool>(), string.Empty, false, false, null,
          It.IsAny<IBasicConsumer>()))
        .Returns("test tag")
        .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
          (_1, _2, _3, _4, _5, _6, consumer) =>
          {
            Consumer = consumer as AsyncEventingBasicConsumer;
            Consumer.HandleBasicConsumeOk("test tag");
          });
      if (_getResult != null)
      {
        ChannelMock
          .Setup(m => m.BasicGet(It.IsAny<string>(), It.IsAny<bool>()))
          .Returns(_getResult);
        ChannelMock
          .Setup(m => m.BasicConsume(It.IsAny<string>(), It.IsAny<bool>(), string.Empty, false, false, null,
            It.IsAny<IBasicConsumer>()))
          .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
            (_1, _2, _3, _4, _5, _6, consumer) =>
            {
              (consumer as AsyncEventingBasicConsumer).HandleBasicDeliver(null, 0, false, null, null,
                _getResult.BasicProperties, _getResult.Body);
            });
      }

      ConnectionMock
        .Setup(m => m.CreateModel())
        .Returns(ChannelMock.Object);

      return ConnectionMock.Object;
    }

    public ConnectionFactoryMock ReceivesMessage(IBasicProperties msgProperties, byte[] body)
    {
      _getResult = new BasicGetResult(
        0,
        false,
        null,
        null,
        1,
        msgProperties,
        new ReadOnlyMemory<byte>(body));
      return this;
    }
  }
}