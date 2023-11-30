using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Factories.Impl;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  /// <inheritdoc />
  public class RabbitMqQueueMessageAdapterConstructor : QueueMessageAdapterConstructorBase<RabbitMqConfiguration,
    RabbitMqQueueConfiguration>
  {
    private readonly RabbitMqConnectionFactory _connectionFactory;
    private readonly ILoggerFactory _loggerFactory;

    /// <inheritdoc />
    public RabbitMqQueueMessageAdapterConstructor(
      IConfiguration configuration,
      ILoggerFactory loggerFactory) :
      base(configuration, "rabbitmq")
    {
      _loggerFactory = loggerFactory;
      _connectionFactory = new RabbitMqConnectionFactory();
    }

    /// <inheritdoc />
    protected override IMessageAdapter CreateAdapter(RabbitMqQueueConfiguration queueConfiguration)
    {
      return new RabbitMqQueueMessageAdapter(
        queueConfiguration,
        _connectionFactory,
        _loggerFactory.CreateLogger<RabbitMqQueueMessageAdapter>());
    }
  }
}