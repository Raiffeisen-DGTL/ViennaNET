using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.KafkaQueue.DefaultConfiguration
{
  /// <inheritdoc />
  public class KafkaPackage : IPackage
  {
    /// <summary>
    /// Пакет SimpleInjector для работы с Messaging
    /// </summary>
    public void RegisterServices(Container container)
    {
      container.Collection.Append<IMessageAdapterConstructor, KafkaQueueMessageAdapterConstructor>(Lifestyle.Singleton);
    }
  }
}
