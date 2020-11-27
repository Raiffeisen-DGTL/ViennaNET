using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  /// <inheritdoc />
  public class RabbitMqQueueMessageAdapterConstructor : QueueMessageAdapterConstructorBase<RabbitMqConfiguration,
    RabbitMqQueueConfiguration>
  {
    private readonly ILoggerFactory _loggerFactory;
    private readonly IAdvancedBusFactory _advancedBusFactory;

    /// <inheritdoc />
    public RabbitMqQueueMessageAdapterConstructor(
      IAdvancedBusFactory advancedBusFactory, 
      IConfiguration configuration,
      ILoggerFactory loggerFactory) :
      base(configuration, "rabbitmq")
    {
      _loggerFactory = loggerFactory;
      _advancedBusFactory = advancedBusFactory.ThrowIfNull(nameof(advancedBusFactory));
    }

    /// <inheritdoc />
    protected override IMessageAdapter CreateAdapter(RabbitMqQueueConfiguration queueConfiguration)
    {
      return new RabbitMqQueueMessageAdapter(
        _advancedBusFactory, 
        queueConfiguration,
        _loggerFactory.CreateLogger<RabbitMqQueueMessageAdapter>());
    }

    /// <inheritdoc />
    protected override void CheckConfigurationParameters(RabbitMqQueueConfiguration configuration)
    {
      configuration.Id.ThrowIfNullOrWhiteSpace(nameof(configuration.Id));
      configuration.Server.ThrowIfNullOrWhiteSpace(nameof(configuration.Server));
      configuration.ExchangeName.ThrowIfNullOrWhiteSpace(nameof(configuration.ExchangeName));
      configuration.IntervalPollingQueue.ThrowIf(value => value <= 0, nameof(configuration.IntervalPollingQueue));
      configuration.User.ThrowIfNullOrWhiteSpace(nameof(configuration.User));
      configuration.Password.ThrowIfNullOrWhiteSpace(nameof(configuration.Password));
    }
  }
}