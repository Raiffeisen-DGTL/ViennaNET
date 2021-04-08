using System;
using NUnit.Framework;
using ViennaNET.Messaging.ActiveMQQueue.Tests.DSL;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(ActiveMqQueueMessageAdapter))]
  public class ActiveMqQueueMessageAdapterTests
  {
    public static readonly object[] ReceiveTestCaseSource =
    {
      (object)null, TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.FromSeconds(5)
    };

    [Test]
    public void ConfigurationTest()
    {
      var configuration = new ActiveMqQueueConfiguration
      {
        IntervalPollingQueue = 1234,
        ProcessingType = MessageProcessingType.Subscribe
      };

      var adapter = Given.MessageAdapter.WithConfiguration(configuration).Please();

      Assert.AreEqual(configuration, adapter.Configuration);
    }

    [Test]
    public void ConnectedAfterConnectTest()
    {
      var adapter = Given.MessageAdapter.Please();

      adapter.Connect();

      Assert.IsTrue(adapter.IsConnected);
    }

    [Test]
    public void DisconnectedAfterDisconnectTest()
    {
      var adapter = Given.MessageAdapter.Please();

      adapter.Connect();
      adapter.Disconnect();

      Assert.IsFalse(adapter.IsConnected);
    }

    [Test]
    public void ListenQueueNameTest()
    {
      var queueName = "qwe";
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given
        .MessageAdapter
        .WithConnectionFactoryProvider(provider.Please())
        .WithConfiguration(new ActiveMqQueueConfiguration { UseQueueString = false, QueueName = queueName })
        .Please();

      adapter.Connect();
      adapter.TryReceive(out var msg);

      provider.SessionMock.Verify(x => x.GetQueue(queueName));
    }

    [Test]
    public void ListenQueueTest()
    {
      var queueName = "qwe";
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given
        .MessageAdapter
        .WithConnectionFactoryProvider(provider.Please())
        .WithConfiguration(new ActiveMqQueueConfiguration { UseQueueString = true, QueueString = $"queue://{queueName}" })
        .Please();

      adapter.Connect();
      adapter.TryReceive(out var msg);

      provider.SessionMock.Verify(x => x.GetQueue(queueName));
    }

    [Test]
    public void ListenTopicTest()
    {
      var topicName = "qwe";
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given
        .MessageAdapter
        .WithConnectionFactoryProvider(provider.Please())
        .WithConfiguration(new ActiveMqQueueConfiguration { UseQueueString = true, QueueString = $"topic://{topicName}" })
        .Please();

      adapter.Connect();
      adapter.TryReceive(out var msg);

      provider.SessionMock.Verify(x => x.GetTopic(topicName));
    }

    [Test]
    public void ReceiveDisposedTest()
    {
      var adapter = Given.MessageAdapter.Please();

      adapter.Dispose();
      Assert.Throws<ObjectDisposedException>(() => adapter.Receive());
    }

    [Test]
    public void ReceiveNotConnectedTest()
    {
      var adapter = Given.MessageAdapter.Please();

      var ex = Assert.Throws<MessagingException>(() => adapter.Receive());
      StringAssert.Contains("Connection is not open", ex.Message);
    }

    [Test]
    [TestCaseSource(nameof(ReceiveTestCaseSource))]
    public void ReceiveTest(TimeSpan? timeout)
    {
      var adapter = Given
        .MessageAdapter
        .ConfigureConnectionFactoryProvider(b => b.WithMessageProducer(Given.MessageProducer("1", "2")).Please())
        .Please();

      adapter.Connect();
      var msg = adapter.Receive(null, timeout);

      Assert.IsNotNull(msg);
      Assert.AreEqual("1", msg.MessageId);
      Assert.AreEqual("2", msg.CorrelationId);
    }

    [Test]
    public void ReceiveWithSelectorTest()
    {
      const string testCorrelationId = "1";
      const string testMessageId = "2";
      var adapter = Given
        .MessageAdapter
        .ConfigureConnectionFactoryProvider(b =>
          b.WithMessageProducer(Given.MessageProducer(testCorrelationId, testMessageId)).Please())
        .WithConfiguration(new ActiveMqQueueConfiguration { Selector = $"JMSMessageID = {testMessageId}" })
        .Please();

      adapter.Connect();
      var msg = adapter.Receive(testCorrelationId, TimeSpan.MinValue);

      Assert.IsNotNull(msg);
      Assert.AreEqual(testCorrelationId, msg.MessageId);
      Assert.AreEqual(testMessageId, msg.CorrelationId);
    }

    [Test]
    public void ReturnFalseFromTryReceiveOnNoMessageTest()
    {
      var adapter = Given.MessageAdapter.Please();

      adapter.Connect();
      var retVal = adapter.TryReceive(out var message);

      Assert.IsFalse(retVal);
      Assert.IsNull(message);
    }

    [Test]
    public void SendTest()
    {
      var adapter = Given
        .MessageAdapter
        .ConfigureConnectionFactoryProvider(b => b.WithMessageSender(Given.MessageSender("1", "2")).Please())
        .Please();

      adapter.Connect();
      var retVal = adapter.Send(new TextMessage { Body = "qwe" });

      Assert.IsNotNull(retVal);
      Assert.AreEqual("1", retVal.MessageId);
      Assert.AreEqual("2", retVal.CorrelationId);
    }

    [Test]
    public void ThrowExceptionInReceiveOnNoMessageTest()
    {
      var adapter = Given.MessageAdapter.Please();

      adapter.Connect();

      Assert.Throws<MessageDidNotReceivedException>(() => adapter.Receive());
    }
  }
}