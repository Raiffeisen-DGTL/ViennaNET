using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.MQSeriesQueue.DefaultConfiguration
{
  /// <inheritdoc />
  public class MqSeriesPackage : IPackage
  {
    /// <summary>
    ///   Пакет SimpleInjector для работы с Messaging
    /// </summary>
    public void RegisterServices(Container container)
    {
      container.Collection.Append<IMessageAdapterConstructor, MqSeriesQueueMessageAdapterConstructor>(Lifestyle.Singleton);
    }
  }
}