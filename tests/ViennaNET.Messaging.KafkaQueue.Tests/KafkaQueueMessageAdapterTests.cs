﻿using Confluent.Kafka;
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
            var config = new KafkaQueueConfiguration { ConsumerConfig = new ConsumerConfig() };
            var connectionFactoryMock = new Mock<IKafkaConnectionFactory> { DefaultValue = DefaultValue.Mock };
            var adapter = Given.KafkaQueueMessageAdapter
                .WithConfiguration(config)
                .WithConnectionFactory(connectionFactoryMock.Object)
                .Please();

            adapter.Connect();

            connectionFactoryMock.Verify(x
                => x.CreateConsumer(config, It.IsAny<Action<IConsumer<byte[], byte[]>, LogMessage>>(),
                    It.IsAny<Action<IConsumer<byte[], byte[]>, Error>>()));

            Assert.That(adapter.IsConnected, Is.True);
        }

        [Test]
        public void Connect_Producer_CreateProducerCalled()
        {
            var config = new KafkaQueueConfiguration { ProducerConfig = new ProducerConfig() };
            var connectionFactoryMock = new Mock<IKafkaConnectionFactory> { DefaultValue = DefaultValue.Mock };
            var adapter = Given.KafkaQueueMessageAdapter
                .WithConfiguration(config)
                .WithConnectionFactory(connectionFactoryMock.Object)
                .Please();

            adapter.Connect();

            connectionFactoryMock.Verify(x
                => x.CreateProducer(config, It.IsAny<Action<IProducer<byte[], byte[]>, LogMessage>>(),
                    It.IsAny<Action<IProducer<byte[], byte[]>, Error>>()));

            Assert.That(adapter.IsConnected, Is.True);
        }

        [Test]
        public void Ctor_ConfigIsNotNull_DoesNotThrowException()
        {
            _ = Given.KafkaQueueMessageAdapter.Please();
            Assert.Pass();
        }

        [Test]
        public void Disconnect_Producer_NotConnected()
        {
            var config = new KafkaQueueConfiguration { ProducerConfig = new ProducerConfig() };
            var connectionFactoryMock = new Mock<IKafkaConnectionFactory>();
            var adapter = Given.KafkaQueueMessageAdapter
                .WithConfiguration(config)
                .WithConnectionFactory(connectionFactoryMock.Object)
                .Please();

            adapter.Connect();
            adapter.Disconnect();

            Assert.That(adapter.IsConnected, Is.False);
        }

        [Test]
        public void Receive_EmptyQueue_ThrowsException()
        {
            var config = new KafkaQueueConfiguration { ConsumerConfig = new ConsumerConfig() };
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
            var config = new KafkaQueueConfiguration { ProducerConfig = new ProducerConfig(), QueueName = "QueueName" };
            var connectionFactoryMock = Given.KafkaConnectionFactory;
            var adapter = Given.KafkaQueueMessageAdapter
                .WithConfiguration(config)
                .WithConnectionFactory(connectionFactoryMock)
                .Please();

            adapter.Connect();
            adapter.Send(new TextMessage { Body = string.Empty });

            connectionFactoryMock.ProducerMock.Verify(
                x => x.Produce(config.QueueName, It.IsAny<Message<byte[], byte[]>>(),
                    It.IsAny<Action<DeliveryReport<byte[], byte[]>>>()));
        }

        [Test]
        public void TryReceive_EmptyQueue_ReturnsFalse()
        {
            var config = new KafkaQueueConfiguration { ConsumerConfig = new ConsumerConfig() };
            var connectionFactoryMock = Given.KafkaConnectionFactory;
            var adapter = Given.KafkaQueueMessageAdapter
                .WithConfiguration(config)
                .WithConnectionFactory(connectionFactoryMock)
                .Please();

            adapter.Connect();
            var hasMessage = adapter.TryReceive(out var message, null!, TimeSpan.MaxValue);

            Assert.Multiple(() =>
            {
                Assert.That(hasMessage, Is.False);
                Assert.That(message, Is.Null);
                connectionFactoryMock.ConsumerMock.Verify(x => x.Consume(default(CancellationToken)));
            });
        }

        [Test]
        public void TryReceive_HasMessage_ReturnsTrueAndMessage()
        {
            var config = new KafkaQueueConfiguration { ConsumerConfig = new ConsumerConfig() };
            var consumeResult = new ConsumeResult<byte[], byte[]>
            {
                Message = new Message<byte[], byte[]>
                {
                    Value = [0xDE, 0xAD, 0xBE, 0xEF],
                    Timestamp = new Timestamp(DateTime.Now),
                    Headers = []
                },
                Topic = "Topic"
            };
            var connectionFactoryMock = Given.KafkaConnectionFactory;
            var timeout = TimeSpan.FromSeconds(1);
            var adapter = Given.KafkaQueueMessageAdapter
                .WithConfiguration(config)
                .WithConnectionFactory(connectionFactoryMock)
                .Please();
            
            connectionFactoryMock.ConsumerMock.Setup(x => x.Consume(timeout)).Returns(consumeResult);

            adapter.Connect();
            var hasMessage = adapter.TryReceive(out var message, null!, timeout);
            var bytesMessage = (BytesMessage)message;

            Assert.Multiple(() =>
            {
                Assert.That(hasMessage, Is.True);
                Assert.That(message, Is.InstanceOf<BytesMessage>());
                Assert.That(consumeResult.Message.Value, Is.EqualTo(bytesMessage.Body));
                Assert.That(consumeResult.Message.Timestamp.UtcDateTime, Is.EqualTo(bytesMessage.SendDateTime));
                Assert.That(consumeResult.Topic, Is.EqualTo(bytesMessage.ReplyQueue));
            });
        }
    }
}