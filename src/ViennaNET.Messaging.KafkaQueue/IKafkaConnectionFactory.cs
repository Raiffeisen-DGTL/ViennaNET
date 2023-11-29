using System;
using Confluent.Kafka;

namespace ViennaNET.Messaging.KafkaQueue
{
  internal interface IKafkaConnectionFactory
  {
    IConsumer<byte[], byte[]> CreateConsumer(
      KafkaQueueConfiguration config,
      Action<IConsumer<byte[], byte[]>, LogMessage> logHandler = null,
      Action<IConsumer<byte[], byte[]>, Error> errorHandler = null);

    IProducer<byte[], byte[]> CreateProducer(
      KafkaQueueConfiguration config,
      Action<IProducer<byte[], byte[]>, LogMessage> logHandler = null,
      Action<IProducer<byte[], byte[]>, Error> errorHandler = null);
  }
}