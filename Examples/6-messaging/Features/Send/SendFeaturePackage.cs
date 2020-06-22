using EmptyService.Features.Send.Messages;
using EmptyService.Features.Send.Services;
using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.Tools;

namespace EmptyService.Features.Send
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
