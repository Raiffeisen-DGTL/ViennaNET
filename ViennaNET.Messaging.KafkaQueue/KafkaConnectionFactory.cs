using System;
using Confluent.Kafka;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.KafkaQueue
{
  internal class KafkaConnectionFactory : IKafkaConnectionFactory
  {
    public IConsumer<Ignore, byte[]> CreateConsumer(
      KafkaQueueConfiguration config,
      Action<IConsumer<Ignore, byte[]>, LogMessage> logHandler = null,
      Action<IConsumer<Ignore, byte[]>, Error> errorHandler = null)
    {
      config.ThrowIfNull(nameof(config));

      var builder = new ConsumerBuilder<Ignore, byte[]>(new ConsumerConfig
      {
        GroupId = config.GroupId,
        BootstrapServers = $"{config.Server}",
        AutoOffsetReset = config.AutoOffsetReset,
        SaslKerberosKeytab = config.KeyTab,
        SaslKerberosPrincipal = config.User,
        SaslKerberosServiceName = config.ServiceName,
        SecurityProtocol = config.Protocol,
        SaslMechanism = config.Mechanism,
        Debug = config.Debug
      });

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

    public IProducer<Null, byte[]> CreateProducer(
      KafkaQueueConfiguration config,
      Action<IProducer<Null, byte[]>, LogMessage> logHandler = null,
      Action<IProducer<Null, byte[]>, Error> errorHandler = null)
    {
      config.ThrowIfNull(nameof(config));

      var builder = new ProducerBuilder<Null, byte[]>(new ProducerConfig
      {
        BootstrapServers = $"{config.Server}",
        SaslKerberosKeytab = config.KeyTab,
        SaslKerberosPrincipal = config.User,
        SaslKerberosServiceName = config.ServiceName,
        SecurityProtocol = config.Protocol,
        SaslMechanism = config.Mechanism,
        Debug = config.Debug
      });

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
