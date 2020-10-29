using System;
using Confluent.Kafka;
using Moq;
using ViennaNET.Messaging.KafkaQueue;

namespace ViennaNET.Messaging.Tests.Unit.Kafka.DSL
{
  internal class KafkaConnectionFactoryMock : IKafkaConnectionFactory
  {
    public Mock<IConsumer<Ignore, byte[]>> ConsumerMock { get; } = new Mock<IConsumer<Ignore, byte[]>>();

    public Mock<IProducer<Null, byte[]>> ProducerMock { get; } = new Mock<IProducer<Null, byte[]>>();

    public IConsumer<Ignore, byte[]> CreateConsumer(KafkaQueueConfiguration config, Action<IConsumer<Ignore, byte[]>, LogMessage> logHandler, Action<IConsumer<Ignore, byte[]>, Error> errorHandler)
    {
      return ConsumerMock.Object;
    }

    public IProducer<Null, byte[]> CreateProducer(KafkaQueueConfiguration config, Action<IProducer<Null, byte[]>, LogMessage> logHandler, Action<IProducer<Null, byte[]>, Error> errorHandler)
    {
      return ProducerMock.Object;
    }
  }
}
