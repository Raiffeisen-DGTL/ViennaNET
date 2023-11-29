using System.Collections.Generic;
using Moq;
using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;

namespace ViennaNET.WebApi.Configurators.Diagnostic.Tests.Unit.DSL
{
  internal class HealthCheckingServiceBuilder
  {
    private readonly Mock<IHealthCheckingService> _mock = new();

    public HealthCheckingServiceBuilder WithCheckResult(IEnumerable<DiagnosticInfo> results)
    {
      _mock.Setup(x => x.CheckHealthAsync()).ReturnsAsync(results);
      return this;
    }

    public IHealthCheckingService Build()
    {
      return _mock.Object;
    }
  }
}