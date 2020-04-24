using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Processing;
using ViennaNET.Messaging.Processing.Impl;
using ViennaNET.Messaging.Tools;
using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Diagnostic;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Messaging.Resources;
using ViennaNET.Messaging.Resources.Impl;
using ViennaNET.CallContext;

namespace ViennaNET.Messaging.DefaultConfiguration
{
  /// <inheritdoc />
  public class MessagingPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      container.RegisterSingleton<IMessagingCallContextAccessor, MessagingCallContextAccessor>();
      container.Collection.Append<ICallContextAccessor, MessagingCallContextAccessor>(Lifestyle.Singleton);

      container.Register<IMessageAdapterFactory, MessageAdapterFactory>(Lifestyle.Singleton);
      container.Register<IMessagingComponentFactory, MessagingComponentFactory>(Lifestyle.Singleton);

      container.Collection.Append<IMessageSerializer, PlainTextSerializer>(Lifestyle.Singleton);
      container.Collection.Append<IMessageDeserializer, PlainTextSerializer>(Lifestyle.Singleton);

      container.Register<IQueueReactorFactory, QueueReactorFactory>(Lifestyle.Singleton);
      container.Collection.Register<IProcessor>(GetType().Assembly);
      container.Collection.Register<IProcessorAsync>(GetType().Assembly);

      container.Collection.Append<IDiagnosticImplementor, MessagingConnectionChecker>(Lifestyle.Singleton);

      container.Collection.Register<IResourcesStorage>(GetType().Assembly);
      container.Register<IResourcesLoader, EmbeddedResourcesLoader>(Lifestyle.Singleton);
      container.Register<IResourcesProvider, ResourcesProvider>(Lifestyle.Singleton);
    }
  }
}
