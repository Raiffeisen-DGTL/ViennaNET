using Microsoft.Extensions.Configuration;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.KafkaQueue
{
  /// <inheritdoc />
  public class KafkaQueueMessageAdapterConstructor : QueueMessageAdapterConstructorBase<KafkaConfiguration, KafkaQueueConfiguration>
  {
    /// <inheritdoc />
    public KafkaQueueMessageAdapterConstructor(IConfiguration configuration) : base(configuration, "kafka")
    {
    }

    /// <inheritdoc />
    protected override IMessageAdapter CreateAdapter(KafkaQueueConfiguration queueConfiguration, bool isDiagnostic)
    {
      return new KafkaQueueMessageAdapter(queueConfiguration, isDiagnostic);
    }

    /// <inheritdoc />
    protected override void CheckConfigurationParameters(KafkaQueueConfiguration configuration)
    {
      configuration.Id.ThrowIfNullOrWhiteSpace(nameof(configuration.Id));
      configuration.Server.ThrowIfNullOrWhiteSpace(nameof(configuration.Server));
    }
  }
}