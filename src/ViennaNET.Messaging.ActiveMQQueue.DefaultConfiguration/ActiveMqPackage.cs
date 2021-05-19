using System.Diagnostics.CodeAnalysis;
using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.ActiveMQQueue.DefaultConfiguration
{
  /// <summary>
  ///   Package for register required services for activemq adapter
  /// </summary>
  [ExcludeFromCodeCoverage]
  public class ActiveMqPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      container.RegisterSingleton<IActiveMqConnectionFactory, ActiveMqConnectionFactory>();
      container.Collection.Append<IMessageAdapterConstructor, ActiveMqQueueMessageAdapterConstructor>(
        Lifestyle.Singleton);
    }
  }
}