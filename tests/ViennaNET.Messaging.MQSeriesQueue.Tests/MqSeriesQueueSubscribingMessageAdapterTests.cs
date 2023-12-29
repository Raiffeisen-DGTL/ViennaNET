using IBM.XMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue.Tests.DSL;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests;

[TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueSubscribingMessageAdapter))]
public class MqSeriesQueueSubscribingMessageAdapterTests
{
    public static readonly object[] ReceiveTestCaseSource =
    {
        null!, TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.FromSeconds(5)
    };

    [Test]
    public void ConfigurationTest()
    {
        var configuration = new MqSeriesQueueConfiguration
        {
            IntervalPollingQueue = 1234, ProcessingType = MessageProcessingType.Subscribe
        };

        var adapter = Given.SubscribingMessageAdapter.WithConfiguration(configuration).Please();

        Assert.That(configuration, Is.EqualTo(adapter.Configuration));
    }

    [Test]
    public void ConnectedAfterConnectTest()
    {
        var adapter = Given.SubscribingMessageAdapter.Please();

        adapter.Connect();

        Assert.That(adapter.IsConnected, Is.True);
    }

    [Test]
    public void DisconnectedAfterDisconnectTest()
    {
        var adapter = Given.SubscribingMessageAdapter.Please();

        adapter.Connect();
        adapter.Disconnect();

        Assert.That(adapter.IsConnected, Is.False);
    }

    [Test]
    public void ListenQueueNameTest()
    {
        const string queueName = "qwe";
        var provider = Given.ConnectionFactoryProvider;
        var adapter = Given
            .SubscribingMessageAdapter
            .WithConnectionFactoryProvider(provider.Please())
            .WithConfiguration(new MqSeriesQueueConfiguration { UseQueueString = false, QueueName = queueName })
            .Please();

        adapter.Connect();
        adapter.TryReceive(out _);

        provider.SessionMock.Verify(x => x.CreateQueue($"queue:///{queueName}"));
    }

    [Test]
    public void ListenQueueTest()
    {
        const string queue = "queue:///qwe";
        var provider = Given.ConnectionFactoryProvider;
        var adapter = Given
            .SubscribingMessageAdapter
            .WithConnectionFactoryProvider(provider.Please())
            .WithConfiguration(new MqSeriesQueueConfiguration { UseQueueString = true, QueueString = queue })
            .Please();

        adapter.Connect();
        adapter.TryReceive(out _);

        provider.SessionMock.Verify(x => x.CreateQueue(queue));
    }

    [Test]
    public void ListenTopicTest()
    {
        const string topic = "topic:///qwe";
        var provider = Given.ConnectionFactoryProvider;
        var adapter = Given
            .SubscribingMessageAdapter
            .WithConnectionFactoryProvider(provider.Please())
            .WithConfiguration(new MqSeriesQueueConfiguration { UseQueueString = true, QueueString = topic })
            .Please();

        adapter.Connect();
        adapter.TryReceive(out _);

        provider.SessionMock.Verify(x => x.CreateTopic(topic));
    }

    [Test]
    public void ReceiveDisposedTest()
    {
        var adapter = Given.SubscribingMessageAdapter.Please();

        adapter.Dispose();
        Assert.Throws<ObjectDisposedException>(() => adapter.Receive());
    }

    [Test]
    public void ReceiveNotConnectedTest()
    {
        Assert.That(() => Given.SubscribingMessageAdapter.Please().Receive(),
            Throws.InstanceOf<MessagingException>().And.Message.Contains("Connection is not open"));
    }

    [Test]
    [TestCaseSource(nameof(ReceiveTestCaseSource))]
    public void ReceiveTest(TimeSpan? timeout)
    {
        const string messageId = "1";
        const string correlationId = "2";
        var adapter = Given.SubscribingMessageAdapter
            .ConfigureConnectionFactoryProvider(
                b => b.WithMessageProducer(Given.MessageProducer(messageId, correlationId)).Please())
            .Please();

        adapter.Connect();
        var msg = adapter.Receive(null!, timeout);

        Assert.Multiple(() =>
        {
            Assert.That(msg, Is.Not.Null);
            Assert.That(msg.MessageId, Is.EqualTo(messageId));
            Assert.That(msg.CorrelationId, Is.EqualTo(correlationId));
        });
    }

    [Test]
    public void ReceiveWithSelectorTest()
    {
        const string messageId = "1";
        const string correlationId = "2";
        var adapter = Given.SubscribingMessageAdapter
            .ConfigureConnectionFactoryProvider(b =>
                b.WithMessageProducer(Given.MessageProducer(messageId, correlationId)).Please())
            .WithConfiguration(new MqSeriesQueueConfiguration { Selector = $"JMSMessageID = {messageId}" })
            .Please();

        adapter.Connect();
        var msg = adapter.Receive(correlationId, TimeSpan.MinValue);

        Assert.Multiple(() =>
        {
            Assert.That(msg, Is.Not.Null);
            Assert.That(msg.MessageId, Is.EqualTo(messageId));
            Assert.That(msg.CorrelationId, Is.EqualTo(correlationId));
        });
    }

    [Test]
    public void ReturnFalseFromTryReceiveOnNoMessageTest()
    {
        var adapter = Given.SubscribingMessageAdapter.Please();

        adapter.Connect();
        var retVal = adapter.TryReceive(out var message);

        Assert.Multiple(() =>
        {
            Assert.That(retVal, Is.False);
            Assert.That(message, Is.Null);
        });
    }

    [Test]
    public void SendTest()
    {
        const string messageId = "1";
        const string correlationId = "2";
        var adapter = Given
            .SubscribingMessageAdapter
            .ConfigureConnectionFactoryProvider(
                b => b.WithMessageSender(Given.MessageSender(messageId, correlationId)).Please())
            .Please();

        adapter.Connect();
        var retVal = adapter.Send(new TextMessage { Body = "qwe" });

        Assert.Multiple(() =>
        {
            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.MessageId, Is.EqualTo(messageId));
            Assert.That(retVal.CorrelationId, Is.EqualTo(correlationId));
        });
    }

    [Test]
    public void SubscribeTest()
    {
        var provider = Given.ConnectionFactoryProvider;
        var adapter = Given.SubscribingMessageAdapter.WithConnectionFactoryProvider(provider.Please()).Please();

        adapter.Connect();
        adapter.Subscribe(_ => Task.CompletedTask);

        provider.MessageConsumerMock.VerifySet(x => x.MessageListener = It.IsNotNull<MessageListener>());
    }

    [Test]
    public void ThrowExceptionInReceiveOnNoMessageTest()
    {
        var adapter = Given.SubscribingMessageAdapter.Please();

        adapter.Connect();

        Assert.Throws<MessageDidNotReceivedException>(() => adapter.Receive());
    }

    [Test]
    public void UnsubscribeTest()
    {
        var provider = Given.ConnectionFactoryProvider;
        var adapter = Given.SubscribingMessageAdapter.WithConnectionFactoryProvider(provider.Please()).Please();

        adapter.Connect();
        adapter.Subscribe(_ => Task.CompletedTask);
        adapter.Unsubscribe();

        provider.MessageConsumerMock.VerifySet(x => x.MessageListener = null);
    }
}