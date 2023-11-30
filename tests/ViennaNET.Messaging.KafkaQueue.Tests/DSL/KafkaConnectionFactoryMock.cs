using System;
using Confluent.Kafka;
using Moq;

namespace ViennaNET.Messaging.KafkaQueue.Tests.DSL
{
  internal class KafkaConnectionFactoryMock : IKafkaConnectionFactory
  {
    public Mock<IConsumer<byte[], byte[]>> ConsumerMock { get; } = new();

    public Mock<IProducer<byte[], byte[]>> ProducerMock { get; } = new();

    public IConsumer<byte[], byte[]> CreateConsumer(KafkaQueueConfiguration config,
      Action<IConsumer<byte[], byte[]>, LogMessage> logHandler, Action<IConsumer<byte[], byte[]>, Error> errorHandler)
    {
      return ConsumerMock.Object;
    }

    public IProducer<byte[], byte[]> CreateProducer(KafkaQueueConfiguration config,
      Action<IProducer<byte[], byte[]>, LogMessage> logHandler, Action<IProducer<byte[], byte[]>, Error> errorHandler)
    {
      return ProducerMock.Object;
    }
  }
}