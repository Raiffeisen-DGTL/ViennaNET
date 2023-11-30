using System;
using Confluent.Kafka;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.KafkaQueue
{
  internal class KafkaConnectionFactory : IKafkaConnectionFactory
  {
    public IConsumer<byte[], byte[]> CreateConsumer(
      KafkaQueueConfiguration config,
      Action<IConsumer<byte[], byte[]>, LogMessage>? logHandler = null,
      Action<IConsumer<byte[], byte[]>, Error>? errorHandler = null)
    {
      config.ThrowIfNull(nameof(config));
      config.ConsumerConfig.ThrowIfNull(nameof(config.ProducerConfig));

      config.ConsumerConfig!.EnableAutoCommit ??= !config.TransactionEnabled;
      config.ConsumerConfig.IsolationLevel ??= config.TransactionEnabled
        ? IsolationLevel.ReadCommitted
        : IsolationLevel.ReadUncommitted;

      var builder = new ConsumerBuilder<byte[], byte[]>(config.ConsumerConfig);

      if (logHandler != null)
      {
        builder.SetLogHandler(logHandler);
      }

      if (errorHandler != null)
      {
        builder.SetErrorHandler(errorHandler);
      }

      return builder.Build();
    }

    public IProducer<byte[], byte[]> CreateProducer(
      KafkaQueueConfiguration config,
      Action<IProducer<byte[], byte[]>, LogMessage>? logHandler = null,
      Action<IProducer<byte[], byte[]>, Error>? errorHandler = null)
    {
      config.ThrowIfNull(nameof(config));
      config.ProducerConfig.ThrowIfNull(nameof(config.ProducerConfig));

      config.ProducerConfig!.TransactionalId = config.TransactionEnabled
        ? config.Id
        : null;

      var builder = new ProducerBuilder<byte[], byte[]>(config.ProducerConfig);

      if (logHandler != null)
      {
        builder.SetLogHandler(logHandler);
      }

      if (errorHandler != null)
      {
        builder.SetErrorHandler(errorHandler);
      }

      return builder.Build();
    }
  }
}