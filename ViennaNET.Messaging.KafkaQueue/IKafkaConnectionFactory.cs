using System;
using Confluent.Kafka;

namespace ViennaNET.Messaging.KafkaQueue
{
  internal interface IKafkaConnectionFactory
  {
    IConsumer<Ignore, byte[]> CreateConsumer(
      KafkaQueueConfiguration config,
      Action<IConsumer<Ignore, byte[]>, LogMessage> logHandler = null,
      Action<IConsumer<Ignore, byte[]>, Error> errorHandler = null);

    IProducer<Null, byte[]> CreateProducer(
      KafkaQueueConfiguration config,
      Action<IProducer<Null, byte[]>, LogMessage> logHandler = null,
      Action<IProducer<Null, byte[]>, Error> errorHandler = null);
  }
}
