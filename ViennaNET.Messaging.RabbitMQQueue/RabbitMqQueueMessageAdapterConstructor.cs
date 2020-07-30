using Microsoft.Extensions.Configuration;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  /// <inheritdoc />
  public class RabbitMqQueueMessageAdapterConstructor : QueueMessageAdapterConstructorBase<RabbitMqConfiguration,
    RabbitMqQueueConfiguration>
  {
    private readonly IAdvancedBusFactory _advancedBusFactory;

    /// <inheritdoc />
    public RabbitMqQueueMessageAdapterConstructor(IAdvancedBusFactory advancedBusFactory, IConfiguration configuration) :
      base(configuration, "rabbitmq")
    {
      _advancedBusFactory = advancedBusFactory.ThrowIfNull(nameof(advancedBusFactory));
    }

    /// <inheritdoc />
    protected override IMessageAdapter CreateAdapter(RabbitMqQueueConfiguration queueConfiguration, bool isDiagnostic)
    {
      return new RabbitMqQueueMessageAdapter(_advancedBusFactory, queueConfiguration, isDiagnostic);
    }

    /// <inheritdoc />
    protected override void CheckConfigurationParameters(RabbitMqQueueConfiguration configuration)
    {
      configuration.Id.ThrowIfNullOrWhiteSpace(nameof(configuration.Id));
      configuration.Server.ThrowIfNullOrWhiteSpace(nameof(configuration.Server));
    }
  }
}