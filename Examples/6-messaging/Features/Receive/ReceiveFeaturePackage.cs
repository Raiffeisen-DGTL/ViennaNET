using MessagingService.Features.Receive.Messages;
using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.Tools;
using ViennaNET.Messaging.DefaultConfiguration;

namespace MessagingService.Features.Receive
{
  public class ReceiveFeaturePackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.RegisterSingleton<Listener>();
      container.RegisterSingleton<IMessageDeserializer<ExampleMessage>, XmlMessageSerializer<ExampleMessage>>();

      container.RegisterAllQueueProcessors(Lifestyle.Singleton, GetType().Assembly);
    }
  }
}
