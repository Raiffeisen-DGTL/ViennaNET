using System;
using Apache.NMS;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.ActiveMQQueue
{
  /// <inheritdoc />
  public class ActiveMqConnectionFactory : IActiveMqConnectionFactory
  {
    private const int DefaultPort = 61616;

    /// <inheritdoc />
    public IConnectionFactory GetConnectionFactory(ActiveMqQueueConfiguration configuration)
    {
      configuration.ThrowIfNull(nameof(configuration));

      Uri connectUri;
      if (configuration.ConnectionString == null)
      {
        var port = configuration.Port.HasValue ? configuration.Port : DefaultPort;
        connectUri = new Uri($"activemq:tcp://{configuration.Server}:{port}");
      }
      else
      {
        connectUri = configuration.ConnectionString;
      }

      if (connectUri.Scheme == "failover")
      {
        // Инициализация фабрики AMQP.ConnectionFactory указана явно из-за бага в Apache.NMS не позволяющего
        // инициализировать failover подклюение по AMQP протоколу строкой вида failover:(amqp://host1,amqp://host2)
        // https://github.com/apache/activemq-nms-api/blob/main/src/nms-api/NMSConnectionFactory.cs
        return new Apache.NMS.AMQP.ConnectionFactory(connectUri);
      }
      else
      {
        return new NMSConnectionFactory(connectUri);
      }
    } 
  }
}