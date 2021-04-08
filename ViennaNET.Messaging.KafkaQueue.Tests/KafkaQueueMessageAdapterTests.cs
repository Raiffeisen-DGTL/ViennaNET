using System;
using System.Threading;
using Confluent.Kafka;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.KafkaQueue.Tests.DSL;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.KafkaQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(KafkaQueueMessageAdapter))]
  public class KafkaQueueMessageAdapterTests
  {
    [Test]
    public void Connect_Consumer_CreateConsumerCalled()
    {
      var config = new KafkaQueueConfiguration {IsConsumer = true};
      var connectionFactoryMock = new Mock<IKafkaConnectionFactory> {DefaultValue = DefaultValue.Mock};
      var adapter = Given.KafkaQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactoryMock.Object)
        .Please();

      adapter.Connect();

      connectionFactoryMock.Verify(x
        => x.CreateConsumer(config, It.IsAny<Action<IConsumer<Ignore, byte[]>, LogMessage>>(),
          It.IsAny<Action<IConsumer<Ignore, byte[]>, Error>>()));
      Assert.IsTrue(adapter.IsConnected);
    }

    [Test]
    public void Connect_Producer_CreateProducerCalled()
    {
      var config = new KafkaQueueConfiguration {IsConsumer = false};
      var connectionFactoryMock = new Mock<IKafkaConnectionFactory> {DefaultValue = DefaultValue.Mock};
      var adapter = Given.KafkaQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactoryMock.Object)
        .Please();

      adapter.Connect();

      connectionFactoryMock.Verify(x
        => x.CreateProducer(config, It.IsAny<Action<IProducer<Null, byte[]>, LogMessage>>(),
          It.IsAny<Action<IProducer<Null, byte[]>, Error>>()));
      Assert.IsTrue(adapter.IsConnected);
    }

    [Test]
    public void Ctor_ConfigIsNotNull_DoesNotThrowException()
    {
      var adapter = Given.KafkaQueueMessageAdapter.Please();
      Assert.Pass();
    }

    [Test]
    public void Disconnect_Producer_NotConnected()
    {
      var config = new KafkaQueueConfiguration {IsConsumer = false};
      var connectionFactoryMock = new Mock<IKafkaConnectionFactory>();
      var adapter = Given.KafkaQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactoryMock.Object)
        .Please();

      adapter.Connect();
      adapter.Disconnect();

      Assert.IsFalse(adapter.IsConnected);
    }

    [Test]
    public void Receive_EmptyQueue_ThrowsException()
    {
      var config = new KafkaQueueConfiguration {IsConsumer = true};
      var connectionFactoryMock = Given.KafkaConnectionFactory;
      var adapter = Given.KafkaQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactoryMock)
        .Please();

      adapter.Connect();
      Assert.Throws<MessageDidNotReceivedException>(() => adapter.Receive());

      connectionFactoryMock.ConsumerMock.Verify(x => x.Consume(TimeSpan.Zero));
    }

    [Test]
    public void Send_Message_ProduceCalled()
    {
      var config = new KafkaQueueConfiguration {IsConsumer = false, QueueName = "QueueName"};
      var connectionFactoryMock = Given.KafkaConnectionFactory;
      var adapter = Given.KafkaQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactoryMock)
        .Please();

      adapter.Connect();
      adapter.Send(new TextMessage {Body = string.Empty});

      connectionFactoryMock.ProducerMock.Verify(
        x => x.Produce(config.QueueName, It.IsAny<Message<Null, byte[]>>(),
          It.IsAny<Action<DeliveryReport<Null, byte[]>>>()));
    }

    [Test]
    public void TryReceive_EmptyQueue_ReturnsFalse()
    {
      var config = new KafkaQueueConfiguration {IsConsumer = true};
      var connectionFactoryMock = Given.KafkaConnectionFactory;
      var adapter = Given.KafkaQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactoryMock)
        .Please();

      adapter.Connect();
      var hasMessage = adapter.TryReceive(out var message, null, TimeSpan.MaxValue);

      Assert.IsFalse(hasMessage);
      Assert.IsNull(message);
      connectionFactoryMock.ConsumerMock.Verify(x => x.Consume(default(CancellationToken)));
    }

    [Test]
    public void TryReceive_HasMessage_ReturnsTrueAndMessage()
    {
      var config = new KafkaQueueConfiguration {IsConsumer = true};
      var consumeResult = new ConsumeResult<Ignore, byte[]>
      {
        Message = new Message<Ignore, byte[]>
        {
          Value = new byte[] {0xDE, 0xAD, 0xBE, 0xEF},
          Timestamp = new Timestamp(DateTime.Now),
          Headers = new Headers()
        },
        Topic = "Topic"
      };
      var connectionFactoryMock = Given.KafkaConnectionFactory;
      var timeout = TimeSpan.FromSeconds(1);
      connectionFactoryMock.ConsumerMock
        .Setup(x => x.Consume(timeout))
        .Returns(consumeResult);
      var adapter = Given.KafkaQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactoryMock)
        .Please();

      adapter.Connect();
      var hasMessage = adapter.TryReceive(out var message, null, timeout);

      Assert.IsTrue(hasMessage);
      Assert.IsInstanceOf<BytesMessage>(message);
      var bytesMessage = (BytesMessage)message;
      Assert.AreEqual(consumeResult.Message.Value, bytesMessage.Body);
      Assert.AreEqual(consumeResult.Timestamp.UtcDateTime, bytesMessage.SendDateTime);
      Assert.AreEqual(consumeResult.Topic, bytesMessage.ReplyQueue);
    }
  }
}