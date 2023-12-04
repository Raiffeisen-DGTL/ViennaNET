using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Mediator.Collectors;
using ViennaNET.Mediator.Pipeline;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator.DefaultConfiguration
{
  /// <summary>
  ///   Пакет SimpleInjector для работы с Mediator
  /// </summary>
  public class MediatorPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      var registration = Lifestyle.Singleton.CreateRegistration<Mediators.Mediator>(container);

      container.AddRegistration<IMediator>(registration);
      container.AddRegistration<IMessageRecipientsRegistrar>(registration);
      container.AddRegistration<IPipelineProcessorsRegistrar>(registration);

      container.Register<IPreProcessorService, PreProcessorService>(Lifestyle.Singleton);
      container.Register<IEventCollectorFactory, EventCollectorFactory>(Lifestyle.Singleton);
      container.Collection.Register<IMessageHandler>(GetType().Assembly);
      container.Collection.Register<IMessageHandlerAsync>(GetType().Assembly);

      container.Register<MediatorRegistrar>(Lifestyle.Singleton);
    }
  }
}