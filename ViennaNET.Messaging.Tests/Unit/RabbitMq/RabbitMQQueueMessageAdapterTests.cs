using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.RabbitMQQueue;
using ViennaNET.Messaging.Tests.Unit.RabbitMq.DSL;

namespace ViennaNET.Messaging.Tests.Unit.RabbitMq
{
  [TestFixture(Category = "Unit", TestOf = typeof(RabbitMqQueueMessageAdapter))]
  public class RabbitMQQueueMessageAdapterTests
  {
    [Test]
    public void Ctor_ConfigIsNotNull_DoesNotThrowException()
    {
      var adapter = Given.RabbitMqQueueMessageAdapter.Please();
      Assert.Pass();
    }

    [TestCase("", "", false)]
    [TestCase("exchange", "", false)]
    [TestCase("", "queue", false)]
    [TestCase("exchange", "queue", true)]
    public void Connect_VariousExchangeAndQueueNames_InitializeBindsExpectedCalled(string exchange, string queue, bool isBindCalled)
    {
      var busFactory = Given.AdvancedBusFactory;
      var config = new RabbitMqQueueConfiguration { ExchangeName = exchange, QueueName = queue };

      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConfiguration(config)
        .WithBusFactory(busFactory)
        .Please();
      adapter.Connect();

      var times = isBindCalled ? Times.AtLeastOnce() : Times.Never();
      busFactory.AdvancedBusMock.Verify(
        x => x.Bind(It.IsAny<IExchange>(), It.IsAny<IQueue>(), It.IsAny<string>()), times);
    }

    [Test]
    public void Connect_ExchangeAndQueueNamesIsNotEmptyListRoutingsIsNotNull_CreateRoutingsBinds()
    {
      var busFactory = Given.AdvancedBusFactory;
      var routings = new [] { "1", "2" };
      var config = new RabbitMqQueueConfiguration { ExchangeName = "exchange", QueueName = "queue", Routings = routings };

      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConfiguration(config)
        .WithBusFactory(busFactory)
        .Please();
      adapter.Connect();

      busFactory.AdvancedBusMock.Verify(
        x => x.Bind(It.IsAny<IExchange>(), It.IsAny<IQueue>(), It.Is<string>(key => key == routings[0])), Times.AtLeastOnce);
      busFactory.AdvancedBusMock.Verify(
        x => x.Bind(It.IsAny<IExchange>(), It.IsAny<IQueue>(), It.Is<string>(key => key == routings[1])), Times.AtLeastOnce);
    }

    [Test]
    public void Send_EmptyMessage_PublishCalled()
    {
      var busFactory = Given.AdvancedBusFactory;
      var config = new RabbitMqQueueConfiguration { Id = "routingKey"};
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConfiguration(config)
        .WithBusFactory(busFactory)
        .Please();
      var message = new TextMessage {Body = "test"};
      var bodyBytes = message.GetMessageBodyAsBytes();

      adapter.Send(message);

      busFactory.AdvancedBusMock.Verify(
        x => x.Publish(
          null, 
          config.Id, 
          false, 
          It.IsAny<MessageProperties>(),
          It.Is<byte[]>(a => a.Where((b, i) => b == bodyBytes[i]).Count() == bodyBytes.Length)));
    }

    [Test]
    public void Receive_QueueEmpty_ThrowsException()
    {
      var busFactory = Given.AdvancedBusFactory;
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithBusFactory(busFactory)
        .Please();

      Assert.Throws<MessageDidNotReceivedException>(() => adapter.Receive());

      busFactory.AdvancedBusMock.Verify(
        x => x.Get(It.IsAny<IQueue>()));
    }

    [Test]
    public void TryReceive_QueueEmpty_ReturnsFalse()
    {
      var busFactory = Given.AdvancedBusFactory;
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithBusFactory(busFactory)
        .Please();

      var hasMessage = adapter.TryReceive(out var message);

      Assert.IsFalse(hasMessage);
      Assert.IsNull(message);
      busFactory.AdvancedBusMock.Verify(
        x => x.Get(It.IsAny<IQueue>()));
    }

    [Test]
    public void Receive_BytesMessage_ReturnBytesMessage()
    {
      var body = new byte[] {0xDE, 0xAD, 0xBE, 0xEF};
      var expiration = new TimeSpan(1, 2, 3);
      var sendDt = DateTime.UtcNow;
      var msgProperties = new MessageProperties
      {
        ContentType = RabbitMqQueueMessageConverter.ContentType.Bytes.ToString("G"),
        MessageId = "messageId", 
        CorrelationId = "correlationId",
        ReplyTo = "replyTo",
        Expiration = expiration.ToString(),
        Timestamp = sendDt.ToFileTimeUtc(),
        Headers = new Dictionary<string, object>()
      };
      var busFactory = Given.AdvancedBusFactory;
      busFactory.AdvancedBusMock
        .Setup(x => x.Get(It.IsAny<IQueue>()))
        .Returns(new BasicGetResult(body, msgProperties, null));
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithBusFactory(busFactory)
        .Please();

      var message = adapter.Receive();

      Assert.AreEqual(msgProperties.MessageId, message.MessageId);
      Assert.AreEqual(msgProperties.CorrelationId, message.CorrelationId);
      Assert.AreEqual(msgProperties.ReplyTo, message.ReplyQueue);
      Assert.AreEqual(expiration, message.LifeTime);
      Assert.AreEqual(sendDt, message.SendDateTime);
      Assert.IsInstanceOf<BytesMessage>(message);
      var bytesMessage = (BytesMessage)message;
      Assert.AreEqual(body, bytesMessage.Body);
    }

    [Test]
    public void Receive_TextMessage_ReturnBytesMessage()
    {
      var body = "body";
      var expiration = new TimeSpan(1, 2, 3);
      var sendDt = DateTime.UtcNow;
      var msgProperties = new MessageProperties
      {
        MessageId = "messageId",
        CorrelationId = "correlationId",
        ReplyTo = "replyTo",
        Expiration = expiration.ToString(),
        Timestamp = sendDt.ToFileTimeUtc(),
        Headers = new Dictionary<string, object>()
      };
      var busFactory = Given.AdvancedBusFactory;
      busFactory.AdvancedBusMock
        .Setup(x => x.Get(It.IsAny<IQueue>()))
        .Returns(new BasicGetResult(Encoding.UTF8.GetBytes(body), msgProperties, null));
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithBusFactory(busFactory)
        .Please();

      var message = adapter.Receive();

      Assert.AreEqual(msgProperties.MessageId, message.MessageId);
      Assert.AreEqual(msgProperties.CorrelationId, message.CorrelationId);
      Assert.AreEqual(msgProperties.ReplyTo, message.ReplyQueue);
      Assert.AreEqual(expiration, message.LifeTime);
      Assert.AreEqual(sendDt, message.SendDateTime);
      Assert.IsInstanceOf<TextMessage>(message);
      var bytesMessage = (TextMessage)message;
      Assert.AreEqual(body, bytesMessage.Body);
    }

    [Test]
    public void Subscribe_NoArgs_ConsumeCalled()
    {
      var busFactory = Given.AdvancedBusFactory;
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithBusFactory(busFactory)
        .Please();

      adapter.Subscribe(null);

      busFactory.AdvancedBusMock.Verify(
        x => x.Consume(null, It.IsAny<Func<byte[], MessageProperties, MessageReceivedInfo, Task>>()));
    }

    [Test]
    public void Reply_TextMessage_PublishCalled()
    {
      var message = new TextMessage {Body = "test", ReplyQueue = "replyQueue"};
      var bodyBytes = message.GetMessageBodyAsBytes();
      var busFactory = Given.AdvancedBusFactory;
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithBusFactory(busFactory)
        .Please();

      adapter.Reply(message);

      busFactory.AdvancedBusMock.Verify(
        x => x.Publish(
          null, 
          message.ReplyQueue, 
          false, 
          It.IsAny<MessageProperties>(), 
          It.Is<byte[]>(a => a.Where((b, i) => b == bodyBytes[i]).Count() == bodyBytes.Length)));
    }
  }
}
