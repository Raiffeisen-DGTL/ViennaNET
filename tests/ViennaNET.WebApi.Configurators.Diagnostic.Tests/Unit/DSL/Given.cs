using ViennaNET.Diagnostic.Data;

namespace ViennaNET.WebApi.Configurators.Diagnostic.Tests.Unit.DSL
{
  internal static class Given
  {
    public static HealthCheckingServiceBuilder HealthCheckingService => new();
    public static DiagnosticInfo HealthyDiagnosticInfo => new("healthy", string.Empty);
    public static DiagnosticInfo UnHealthyDiagnosticInfo => new("unhealthy", string.Empty, DiagnosticStatus.Fail);

    public static DiagnosticInfo UnHealthyDiagnosticInfoWIthSkipFlag =>
      new("unhealthy", string.Empty, DiagnosticStatus.Fail, isSkipResult: true);
  }
}