using System;
using System.Threading.Tasks;
using IBM.XMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.Tests.Unit.MqSeries.DSL;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueSubscribingMessageAdapter))]
  public class MqSeriesQueueSubscribingMessageAdapterTests
  {
    [Test]
    public void ConfigurationTest()
    {
      var configuration = new MqSeriesQueueConfiguration { IntervalPollingQueue = 1234, ProcessingType = MessageProcessingType.Subscribe };

      var adapter = Given.SubscribingMessageAdapter.WithConfiguration(configuration).Please();

      Assert.AreEqual(configuration, adapter.Configuration);
    }

    [Test]
    public void ConnectedAfterConnectTest()
    {
      var adapter = Given.SubscribingMessageAdapter.Please();

      adapter.Connect();

      Assert.IsTrue(adapter.IsConnected);
    }

    [Test]
    public void DisconnectedAfterDisconnectTest()
    {
      var adapter = Given.SubscribingMessageAdapter.Please();

      adapter.Connect();
      adapter.Disconnect();

      Assert.IsFalse(adapter.IsConnected);
    }

    public static readonly object[] ReceiveTestCaseSource = new[]
    {
      (object)null,
      TimeSpan.MinValue,
      TimeSpan.MaxValue,
      TimeSpan.FromSeconds(5)
    };

    [Test]
    [TestCaseSource(nameof(ReceiveTestCaseSource))]
    public void ReceiveTest(TimeSpan? timeout)
    {
      var adapter = Given
        .SubscribingMessageAdapter
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
        .SubscribingMessageAdapter
        .ConfigureConnectionFactoryProvider(b => b.WithMessageProducer(Given.MessageProducer(testCorrelationId, testMessageId)).Please())
        .WithConfiguration(new MqSeriesQueueConfiguration() {Selector = $"JMSMessageID = {testMessageId}" })
        .Please();

      adapter.Connect();
      var msg = adapter.Receive(testCorrelationId, TimeSpan.MinValue);

      Assert.IsNotNull(msg);
      Assert.AreEqual(testCorrelationId, msg.MessageId);
      Assert.AreEqual(testMessageId, msg.CorrelationId);
    }

    [Test]
    public void ReceiveNotConnectedTest()
    {
      var adapter = Given.SubscribingMessageAdapter.Please();

      var ex = Assert.Throws<MessagingException>(() => adapter.Receive());
      StringAssert.Contains("Connection is not open", ex.Message);
    } 
    
    [Test]
    public void ReceiveDisposedTest()
    {
      var adapter = Given.SubscribingMessageAdapter.Please();

      adapter.Dispose();
      Assert.Throws<ObjectDisposedException>(() => adapter.Receive());
    }

    [Test]
    public void ThrowExceptionInReceiveOnNoMessageTest()
    {
      var adapter = Given.SubscribingMessageAdapter.Please();

      adapter.Connect();

      Assert.Throws<MessageDidNotReceivedException>(() => adapter.Receive());
    }

    [Test]
    public void ReturnFalseFromTryReceiveOnNoMessageTest()
    {
      var adapter = Given.SubscribingMessageAdapter.Please();

      adapter.Connect();
      var retVal = adapter.TryReceive(out var message);

      Assert.IsFalse(retVal);
      Assert.IsNull(message);
    }

    [Test]
    public void SendTest()
    {
      var adapter = Given
        .SubscribingMessageAdapter
        .ConfigureConnectionFactoryProvider(b => b.WithMessageSender(Given.MessageSender("1", "2")).Please())
        .Please();

      adapter.Connect();
      var retVal = adapter.Send(new TextMessage { Body = "qwe" });

      Assert.IsNotNull(retVal);
      Assert.AreEqual("1", retVal.MessageId);
      Assert.AreEqual("2", retVal.CorrelationId);
    }

    [Test]
    public void ListenTopicTest()
    {
      var topic = "topic:///qwe";
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given
        .SubscribingMessageAdapter
        .WithConnectionFactoryProvider(provider.Please())
        .WithConfiguration(new MqSeriesQueueConfiguration { UseQueueString = true, QueueString = topic})
        .Please();

      adapter.Connect();
      adapter.TryReceive(out var msg);

      provider.SessionMock.Verify(x => x.CreateTopic(topic));
    }

    [Test]
    public void ListenQueueTest()
    {
      var queue = "queue:///qwe";
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given
        .SubscribingMessageAdapter
        .WithConnectionFactoryProvider(provider.Please())
        .WithConfiguration(new MqSeriesQueueConfiguration { UseQueueString = true, QueueString = queue })
        .Please();

      adapter.Connect();
      adapter.TryReceive(out var msg);

      provider.SessionMock.Verify(x => x.CreateQueue(queue));
    }

    [Test]
    public void ListenQueueNameTest()
    {
      var queueName = "qwe";
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given
        .SubscribingMessageAdapter
        .WithConnectionFactoryProvider(provider.Please())
        .WithConfiguration(new MqSeriesQueueConfiguration { UseQueueString = false, QueueName = queueName })
        .Please();

      adapter.Connect();
      adapter.TryReceive(out var msg);

      provider.SessionMock.Verify(x => x.CreateQueue($"queue:///{queueName}"));
    }

    [Test]
    public void SubscribeTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.SubscribingMessageAdapter.WithConnectionFactoryProvider(provider.Please()).Please();

      adapter.Connect();
      adapter.Subscribe(msg => Task.CompletedTask);
 
      provider.MessageConsumerMock.VerifySet(x => x.MessageListener = It.IsNotNull<MessageListener>());
    }

    [Test]
    public void UnsubscribeTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.SubscribingMessageAdapter.WithConnectionFactoryProvider(provider.Please()).Please();

      adapter.Connect();
      adapter.Subscribe(msg => Task.CompletedTask);
      adapter.Unsubscribe();

      provider.MessageConsumerMock.VerifySet(x => x.MessageListener = null);
    }
  }
}