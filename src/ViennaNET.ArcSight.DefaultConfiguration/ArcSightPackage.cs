using SimpleInjector;
using SimpleInjector.Packaging;

namespace ViennaNET.ArcSight.DefaultConfiguration
{
  /// <inheritdoc />
  /// <summary>
  /// Пакет SimpleInjector для работы с ArcSight
  /// </summary>
  public class ArcSightPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      container.Register<IArcSightClient, ArcSightClient>(Lifestyle.Singleton);
      container.Register<IErrorHandlingPoliciesFactory, ErrorHandlingPoliciesFactory>(Lifestyle.Singleton);
      container.Register<ICefSenderFactory, CefSenderFactory>(Lifestyle.Singleton);
    }
  }
}
