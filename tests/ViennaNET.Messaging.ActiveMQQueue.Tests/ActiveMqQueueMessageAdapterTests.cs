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
        [
            null!, TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.FromSeconds(5)
        ];

        [Test]
        public void ConfigurationTest()
        {
            var configuration = new ActiveMqQueueConfiguration
            {
                IntervalPollingQueue = 1234,
                ProcessingType = MessageProcessingType.Subscribe
            };

            var adapter = Given.MessageAdapter.WithConfiguration(configuration).Please();

            Assert.That(configuration, Is.EqualTo(adapter.Configuration));
        }

        [Test]
        public void ConnectedAfterConnectTest()
        {
            var adapter = Given.MessageAdapter.Please();

            adapter.Connect();

            Assert.That(adapter.IsConnected, Is.True);
        }

        [Test]
        public void DisconnectedAfterDisconnectTest()
        {
            var adapter = Given.MessageAdapter.Please();

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
                .MessageAdapter
                .WithConnectionFactoryProvider(provider.Please())
                .WithConfiguration(new ActiveMqQueueConfiguration { UseQueueString = false, QueueName = queueName })
                .Please();

            adapter.Connect();
            adapter.TryReceive(out _);

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
                .WithConfiguration(new ActiveMqQueueConfiguration
                {
                    UseQueueString = true,
                    QueueString = $"queue://{queueName}"
                })
                .Please();

            adapter.Connect();
            adapter.TryReceive(out _);

            provider.SessionMock.Verify(x => x.GetQueue(queueName));
        }

        [Test]
        public void ListenTopicTest()
        {
            const string topicName = "qwe";
            var provider = Given.ConnectionFactoryProvider;
            var adapter = Given
                .MessageAdapter
                .WithConnectionFactoryProvider(provider.Please())
                .WithConfiguration(new ActiveMqQueueConfiguration
                {
                    UseQueueString = true,
                    QueueString = $"topic://{topicName}"
                })
                .Please();

            adapter.Connect();
            adapter.TryReceive(out _);

            provider.SessionMock.Verify(x => x.GetTopic(topicName));
        }

        [Test]
        public void ReceiveDisposedTest()
        {
            var adapter = Given.MessageAdapter.Please();

            adapter.Dispose();

            Assert.That(() => adapter.Receive(), Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void ReceiveNotConnectedTest()
        {
            Assert.That(() => Given.MessageAdapter.Please().Receive(), Throws.InstanceOf<MessagingException>());
        }

        [Test]
        [TestCaseSource(nameof(ReceiveTestCaseSource))]
        public void ReceiveTest(TimeSpan? timeout)
        {
            const string testCorrelationId = "1";
            const string testMessageId = "2";

            var adapter = Given
                .MessageAdapter
                .ConfigureConnectionFactoryProvider(
                    b => b.WithMessageProducer(Given.MessageProducer(testMessageId, testCorrelationId)).Please())
                .Please();

            adapter.Connect();
            var msg = adapter.Receive(null!, timeout);

            Assert.Multiple(() =>
            {
                Assert.That(msg, Is.Not.Null);
                Assert.That(msg.MessageId, Is.EqualTo(testMessageId));
                Assert.That(msg.CorrelationId, Is.EqualTo(testCorrelationId));
            });
        }

        [Test]
        public void ReceiveWithSelectorTest()
        {
            const string testCorrelationId = "1";
            const string testMessageId = "2";

            var adapter = Given
                .MessageAdapter
                .ConfigureConnectionFactoryProvider(b =>
                    b.WithMessageProducer(Given.MessageProducer(testMessageId, testCorrelationId)).Please())
                .WithConfiguration(new ActiveMqQueueConfiguration { Selector = $"JMSMessageID = {testMessageId}" })
                .Please();

            adapter.Connect();
            var msg = adapter.Receive(testCorrelationId, TimeSpan.MinValue);

            Assert.Multiple(() =>
            {
                Assert.That(msg, Is.Not.Null);
                Assert.That(msg.MessageId, Is.EqualTo(testMessageId));
                Assert.That(msg.CorrelationId, Is.EqualTo(testCorrelationId));
            });
        }

        [Test]
        public void ReturnFalseFromTryReceiveOnNoMessageTest()
        {
            var adapter = Given.MessageAdapter.Please();

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
            const string testCorrelationId = "1";
            const string testMessageId = "2";

            var sender = Given.MessageSender(testMessageId, testCorrelationId);
            var adapter = Given.MessageAdapter
                .ConfigureConnectionFactoryProvider(b => b.WithMessageSender(sender).Please()).Please();

            adapter.Connect();
            var retVal = adapter.Send(new TextMessage { Body = "qwe" });

            Assert.Multiple(() =>
            {
                Assert.That(retVal, Is.Not.Null);
                Assert.That(retVal.MessageId, Is.EqualTo(testMessageId));
                Assert.That(retVal.CorrelationId, Is.EqualTo(testCorrelationId));
            });
        }

        [Test]
        public void ThrowExceptionInReceiveOnNoMessageTest()
        {
            var adapter = Given.MessageAdapter.Please();

            adapter.Connect();

            Assert.That(() => adapter.Receive(), Throws.InstanceOf<MessageDidNotReceivedException>());
        }
    }
}