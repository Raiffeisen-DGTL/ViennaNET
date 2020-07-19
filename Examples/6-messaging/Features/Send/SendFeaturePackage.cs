using MessagingService.Features.Send.Messages;
using MessagingService.Features.Send.Services;
using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.Tools;

namespace MessagingService.Features.Send
{
  public class SendFeaturePackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.Collection.Append<IMessageSerializer, XmlMessageSerializer<ExampleMessage>>(Lifestyle.Singleton);
      container.RegisterSingleton<ISendingService, SendingService>();
    }
  }
}
