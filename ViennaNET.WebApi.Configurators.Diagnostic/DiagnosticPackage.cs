using ViennaNET.Diagnostic;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace ViennaNET.WebApi.Configurators.Diagnosing
{
  public class DiagnosticPackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.Collection.Register<IDiagnosticImplementor>(typeof(DiagnosticPackage).Assembly);
      container.RegisterSingleton<IHealthCheckingService, HealthCheckingService>();
    }
  }
}
