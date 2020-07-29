using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.DefaultConfiguration;
using ViennaNET.Messaging.RabbitMQQueue.DefaultConfiguration;
using ViennaNET.SimpleInjector.Extensions;

namespace MessagingService
{
  public class MessagingServicePackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.AddPackage(new MessagingPackage())
               .AddPackage(new RabbitMqPackage());
    }
  }
}
