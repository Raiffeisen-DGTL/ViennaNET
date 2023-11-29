using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.RabbitMQQueue.Tests.DSL;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(RabbitMqQueueMessageAdapter))]
  public class RabbitMQQueueMessageAdapterTests
  {
    [TestCase("", "", false)]
    [TestCase("exchange", "", false)]
    [TestCase("", "queue", false)]
    [TestCase("exchange", "queue", true)]
    public void Connect_VariousExchangeAndQueueNames_InitializeBindsExpectedCalled(
      string exchange,
      string queue,
      bool isBindCalled)
    {
      var connectionFactory = Given.ConnectionFactoryMock;
      var config = new RabbitMqQueueConfiguration { ExchangeName = exchange, QueueName = queue };

      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactory)
        .Please();
      adapter.Connect();

      var times = isBindCalled ? Times.AtLeastOnce() : Times.Never();
      connectionFactory.ChannelMock.Verify(
        x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), times);
    }

    [Test]
    public void Connect_ExchangeAndQueueNamesIsNotEmptyListRoutingsIsNotNull_CreateRoutingsBinds()
    {
      var connectionFactory = Given.ConnectionFactoryMock;
      var routings = new[] { "1", "2" };
      var config =
        new RabbitMqQueueConfiguration { ExchangeName = "exchange", QueueName = "queue", Routings = routings };

      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactory)
        .Please();
      adapter.Connect();

      Assert.Multiple(() =>
      {
        connectionFactory.ChannelMock.Verify(
          x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(key => key == routings[0]), null),
          Times.AtLeastOnce);
        connectionFactory.ChannelMock.Verify(
          x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(key => key == routings[1]), null),
          Times.AtLeastOnce);
      });
    }

    [TestCase(false, true, true, true, true)]
    [TestCase(false, false, true, true, false)]
    [TestCase(false, true, false, false, true)]
    [TestCase(false, true, true, false, false)]
    [TestCase(true, true, true, true, false)]
    public void IsConnected_VariousState_CorrectlyCalculated(bool isDisposed, bool isChannelOpen, bool isConsumerExists,
      bool isConsumerRunning, bool expected)
    {
      var connectionFactory = Given.ConnectionFactoryMock;
      connectionFactory.ChannelMock.Setup(x => x.IsOpen).Returns(isChannelOpen);
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConnectionFactory(connectionFactory)
        .Please();
      adapter.Connect();
      if (isConsumerExists)
      {
        adapter.Subscribe(_ => Task.CompletedTask);

        if (!isConsumerRunning)
        {
          connectionFactory.Consumer.HandleBasicCancel("");
        }
      }

      if (isDisposed)
      {
        adapter.Dispose();
      }

      var result = adapter.IsConnected;

      Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public void Unsubscribe_OnConsumerCancelled_ChannelCancelCalled(bool consumerCancelled, bool channelCancelledCalled)
    {
      var connectionFactory = Given.ConnectionFactoryMock;
      connectionFactory.ChannelMock.Setup(x => x.IsOpen).Returns(true);
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConnectionFactory(connectionFactory)
        .Please();
      adapter.Connect();
      adapter.Subscribe(_ => Task.CompletedTask);
      if (consumerCancelled)
      {
        connectionFactory.Consumer.HandleBasicCancel("test tag");
      }

      adapter.Unsubscribe();

      connectionFactory.ChannelMock.Verify(x => x.BasicCancel(It.IsAny<string>()),
        channelCancelledCalled
          ? Times.Once
          : Times.Never);
    }

    [Test]
    public void Ctor_ConfigIsNotNull_DoesNotThrowException()
    {
      var adapter = Given.RabbitMqQueueMessageAdapter.Please();
      Assert.Pass();
    }

    public static readonly TimeSpan?[] AllTimeouts =
    {
      null, TimeSpan.MinValue, TimeSpan.MaxValue, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(1)
    };

    [TestCaseSource(nameof(AllTimeouts))]
    public void Receive_BytesMessage_ReturnBytesMessage(TimeSpan? timeout)
    {
      var body = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
      var expiration = new TimeSpan(1, 2, 3);
      var sendDt = DateTime.UtcNow;
      var msgProperties = Given.BasicProperties;
      msgProperties.ContentType = RabbitMqQueueMessageConverter.ContentType.Bytes.ToString("G");
      msgProperties.MessageId = "messageId";
      msgProperties.CorrelationId = "correlationId";
      msgProperties.ReplyTo = "replyTo";
      msgProperties.Expiration = expiration.ToString();
      msgProperties.Timestamp = new AmqpTimestamp(sendDt.ToFileTimeUtc());
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConnectionFactory(Given.ConnectionFactoryMock.ReceivesMessage(msgProperties, body))
        .Please();

      var message = adapter.Receive(timeout: timeout);

      Assert.Multiple(() =>
      {
        Assert.AreEqual(msgProperties.MessageId, message.MessageId);
        Assert.AreEqual(msgProperties.CorrelationId, message.CorrelationId);
        Assert.AreEqual(msgProperties.ReplyTo, message.ReplyQueue);
        Assert.AreEqual(expiration, message.LifeTime);
        Assert.AreEqual(sendDt, message.SendDateTime);
        Assert.IsInstanceOf<BytesMessage>(message);
        var bytesMessage = (BytesMessage)message;
        Assert.AreEqual(body, bytesMessage.Body);
      });
    }

    [Test]
    public void Receive_QueueEmpty_ThrowsException()
    {
      var connectionFactory = Given.ConnectionFactoryMock;
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConnectionFactory(connectionFactory)
        .Please();

      Assert.Throws<MessageDidNotReceivedException>(() => adapter.Receive());

      connectionFactory.ChannelMock.Verify(
        x => x.BasicGet(It.IsAny<string>(), true));
    }

    [TestCaseSource(nameof(AllTimeouts))]
    public void Receive_TextMessage_ReturnTextMessage(TimeSpan? timeout)
    {
      var body = "body";
      var expiration = new TimeSpan(1, 2, 3);
      var sendDt = DateTime.UtcNow;
      var msgProperties = Given.BasicProperties;
      msgProperties.ContentType = RabbitMqQueueMessageConverter.ContentType.Text.ToString("G");
      msgProperties.MessageId = "messageId";
      msgProperties.CorrelationId = "correlationId";
      msgProperties.ReplyTo = "replyTo";
      msgProperties.Expiration = expiration.ToString();
      msgProperties.Timestamp = new AmqpTimestamp(sendDt.ToFileTimeUtc());
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConnectionFactory(Given.ConnectionFactoryMock.ReceivesMessage(msgProperties, Encoding.UTF8.GetBytes(body)))
        .Please();

      var message = adapter.Receive(timeout: timeout);

      Assert.Multiple(() =>
      {
        Assert.AreEqual(msgProperties.MessageId, message.MessageId);
        Assert.AreEqual(msgProperties.CorrelationId, message.CorrelationId);
        Assert.AreEqual(msgProperties.ReplyTo, message.ReplyQueue);
        Assert.AreEqual(expiration, message.LifeTime);
        Assert.AreEqual(sendDt, message.SendDateTime);
        Assert.IsInstanceOf<TextMessage>(message);
        var bytesMessage = (TextMessage)message;
        Assert.AreEqual(body, bytesMessage.Body);
      });
    }

    [Test]
    public void Reply_TextMessage_PublishCalled()
    {
      var message = new TextMessage { Body = "test", ReplyQueue = "replyQueue" };
      var bodyBytes = message.GetMessageBodyAsBytes();
      var connectionFactory = Given.ConnectionFactoryMock;
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConnectionFactory(connectionFactory)
        .Please();

      adapter.Reply(message);

      connectionFactory.ChannelMock.Verify(
        x => x.BasicPublish(
          It.IsAny<string>(),
          message.ReplyQueue,
          false,
          It.IsAny<IBasicProperties>(),
          It.Is<ReadOnlyMemory<byte>>(a => a.Length == bodyBytes.Length)));
    }

    [Test]
    public void Send_EmptyMessage_PublishCalled()
    {
      var connectionFactory = Given.ConnectionFactoryMock;
      var config = new RabbitMqQueueConfiguration { Id = "routingKey" };
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConfiguration(config)
        .WithConnectionFactory(connectionFactory)
        .Please();
      var message = new TextMessage { Body = "test" };
      var bodyBytes = message.GetMessageBodyAsBytes();

      adapter.Send(message);

      connectionFactory.ChannelMock.Verify(
        x => x.BasicPublish(
          It.IsAny<string>(),
          config.Id,
          false,
          It.IsAny<IBasicProperties>(),
          It.Is<ReadOnlyMemory<byte>>(a => a.Length == bodyBytes.Length)));
    }

    public static readonly TimeSpan?[] TryReceive_QueueEmpty_ReturnsFalse_Source =
    {
      null, TimeSpan.MinValue, TimeSpan.FromSeconds(1)
    };

    [TestCaseSource(nameof(TryReceive_QueueEmpty_ReturnsFalse_Source))]
    public void TryReceive_QueueEmpty_ReturnsFalse(TimeSpan? timeout)
    {
      var connectionFactory = Given.ConnectionFactoryMock;
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConnectionFactory(connectionFactory)
        .Please();

      var hasMessage = adapter.TryReceive(out var message, timeout: timeout);

      Assert.IsFalse(hasMessage);
      Assert.IsNull(message);
    }

    [Test]
    public void Subscribe_HasMessage_ShouldCallHandler()
    {
      var body = "body";
      var expiration = new TimeSpan(1, 2, 3);
      var sendDt = DateTime.UtcNow;
      var msgProperties = Given.BasicProperties;
      msgProperties.ContentType = RabbitMqQueueMessageConverter.ContentType.Text.ToString("G");
      msgProperties.MessageId = "messageId";
      msgProperties.CorrelationId = "correlationId";
      msgProperties.ReplyTo = "replyTo";
      msgProperties.Expiration = expiration.ToString();
      msgProperties.Timestamp = new AmqpTimestamp(sendDt.ToFileTimeUtc());
      var connectionFactory = Given.ConnectionFactoryMock.ReceivesMessage(msgProperties, Encoding.UTF8.GetBytes(body));
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConnectionFactory(connectionFactory)
        .Please();
      var handlerMock = new Mock<Func<BaseMessage, Task>>();

      adapter.Subscribe(handlerMock.Object);

      Func<BaseMessage, bool> messageCheck = msg =>
        msg is TextMessage txt
        && txt.Body == body
        && msg.MessageId == msgProperties.MessageId
        && msg.CorrelationId == msgProperties.CorrelationId
        && msg.ReplyQueue == msgProperties.ReplyTo
        && msg.LifeTime == expiration
        && msg.SendDateTime == sendDt;
      handlerMock.Verify(
        m => m(It.Is<BaseMessage>(msg => messageCheck(msg))));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Subscribe_NoAutoAckAndHandlerThrows_ShouldCallReject(bool requeue)
    {
      var connectionFactory = Given.ConnectionFactoryMock.ReceivesMessage(Given.BasicProperties, new byte[0]);
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConfiguration(new RabbitMqQueueConfiguration { Requeue = requeue })
        .WithConnectionFactory(connectionFactory)
        .Please();

      adapter.Subscribe(bm => throw new ApplicationException());

      connectionFactory.ChannelMock.Verify(
        m => m.BasicReject(It.IsAny<ulong>(), requeue));
    }

    [Test]
    public async Task RequestAndWaitResponse_ShouldSendAndReceive()
    {
      var message = new BytesMessage { MessageId = "msgId", Body = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF } };
      var body = "body";
      var msgProperties = Given.BasicProperties;
      msgProperties.ContentType = RabbitMqQueueMessageConverter.ContentType.Text.ToString("G");
      msgProperties.MessageId = "messageId";
      var connectionFactory = Given.ConnectionFactoryMock.ReceivesMessage(msgProperties, Encoding.UTF8.GetBytes(body));
      var adapter = Given.RabbitMqQueueMessageAdapter
        .WithConnectionFactory(connectionFactory)
        .Please();

      var reply = await adapter.RequestAndWaitResponse(message);

      Assert.Multiple(() =>
      {
        connectionFactory.ChannelMock.Verify(
          m => m.BasicPublish(
            null,
            null,
            false,
            It.Is<IBasicProperties>(p => p.MessageId == message.MessageId),
            It.Is<ReadOnlyMemory<byte>>(a => a.Length == message.Body.Length)));
        Assert.IsInstanceOf<TextMessage>(reply);
        var textReply = (TextMessage)reply;
        Assert.AreEqual(body, textReply.Body);
        Assert.AreEqual(msgProperties.MessageId, reply.MessageId);
      });
    }
  }
}