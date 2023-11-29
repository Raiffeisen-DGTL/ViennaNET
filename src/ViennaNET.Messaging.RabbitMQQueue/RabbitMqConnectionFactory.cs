using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  [ExcludeFromCodeCoverage]
  internal class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
  {
    private const string DefaultHost = "localhost";

    public IConnection Create(RabbitMqQueueConfiguration config)
    {
      config.ThrowIfNull(nameof(config));

      var connectionFactory = new ConnectionFactory
      {
        VirtualHost = string.IsNullOrWhiteSpace(config.VirtualHost)
          ? ConnectionFactory.DefaultVHost
          : config.VirtualHost,
        UserName = string.IsNullOrWhiteSpace(config.User) ? ConnectionFactory.DefaultUser : config.User,
        Password = string.IsNullOrWhiteSpace(config.Password) ? ConnectionFactory.DefaultPass : config.Password,
        DispatchConsumersAsync = true,
        RequestedConnectionTimeout = config.ConnectionTimeout
      };

      if (!string.IsNullOrWhiteSpace(config.Addresses))
      {
        return connectionFactory.CreateConnection(AmqpTcpEndpoint.ParseMultiple(config.Addresses));
      }

      connectionFactory.HostName = string.IsNullOrWhiteSpace(config.Server) ? DefaultHost : config.Server;
      connectionFactory.Port = config.Port ?? AmqpTcpEndpoint.UseDefaultPort;
      return connectionFactory.CreateConnection();
    }
  }
}