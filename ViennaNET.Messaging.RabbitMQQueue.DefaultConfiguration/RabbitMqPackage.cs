using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.RabbitMQQueue.DefaultConfiguration
{
  public class RabbitMqPackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.Register<IMessageAdapterConstructor, RabbitMqQueueMessageAdapterConstructor>(Lifestyle.Singleton);
      container.Collection.Append<IMessageAdapterConstructor, RabbitMqQueueMessageAdapterConstructor>(Lifestyle.Singleton);
    }
  }
}