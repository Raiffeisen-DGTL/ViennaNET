using MediatorService.Handlers;
using MediatorService.Messages;
using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Mediator.DefaultConfiguration;
using ViennaNET.SimpleInjector.Extensions;

namespace MediatorService
{
  public class MediatorServicePackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.AddPackage(new MediatorPackage());
      container.RegisterHandler<GetGreetingsRequest, GetGreetingsResult, GetGreetingsHandler>();
    }
  }
}
