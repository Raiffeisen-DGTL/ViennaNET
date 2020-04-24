using Microsoft.Extensions.Configuration;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  /// <inheritdoc />
  public class RabbitMqQueueMessageAdapterConstructor : QueueMessageAdapterConstructorBase<RabbitMqConfiguration,
    RabbitMqQueueConfiguration>
  {
    /// <inheritdoc />
    public RabbitMqQueueMessageAdapterConstructor(IConfiguration configuration) : base(configuration, "rabbitmq")
    {
    }

    /// <inheritdoc />
    protected override IMessageAdapter CreateAdapter(RabbitMqQueueConfiguration queueConfiguration, bool isDiagnostic)
    {
      return new RabbitMqQueueMessageAdapter(queueConfiguration, isDiagnostic);
    }

    /// <inheritdoc />
    protected override void CheckConfigurationParameters(RabbitMqQueueConfiguration configuration)
    {
      configuration.Id.ThrowIfNullOrWhiteSpace(nameof(configuration.Id));
      configuration.Server.ThrowIfNullOrWhiteSpace(nameof(configuration.Server));
    }
  }
}