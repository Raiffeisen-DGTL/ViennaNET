using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.RabbitMQQueue.DefaultConfiguration
{
  /// <summary>
  /// Package for register required services for rabbitmq adapter
  /// </summary>
  public class RabbitMqPackage : IPackage
  {
    /// <summary>
    /// Register services
    /// </summary>
    /// <param name="container"></param>
    public void RegisterServices(Container container)
    {
      container.RegisterSingleton<IAdvancedBusFactory, AdvancedBusFactory>();
      container.Register<IMessageAdapterConstructor, RabbitMqQueueMessageAdapterConstructor>(Lifestyle.Singleton);
      container.Collection.Append<IMessageAdapterConstructor, RabbitMqQueueMessageAdapterConstructor>(Lifestyle.Singleton);
    }
  }
}