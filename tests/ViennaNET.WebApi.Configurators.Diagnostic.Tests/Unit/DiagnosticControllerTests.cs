using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ViennaNET.Diagnostic;
using ViennaNET.WebApi.Configurators.Diagnostic.Tests.Unit.DSL;

namespace ViennaNET.WebApi.Configurators.Diagnostic.Tests.Unit
{
  [TestFixture(Category = "Unit")]
  [TestOf(typeof(DiagnosticController))]
  public class DiagnosticControllerTests
  {
    [Test]
    public async Task ServiceDiagnose_HasHealthyResult_ReturnsOkStatus()
    {
      // arrange
      var healthResult = new[] {Given.HealthyDiagnosticInfo};
      var healthCheckingService = Given.HealthCheckingService.WithCheckResult(healthResult).Build();

      // act
      var controller = new DiagnosticController(healthCheckingService, new NullLogger<DiagnosticController>());
      var result = await controller.ServiceDiagnose();

      // assert
      Assert.That(result, Is.TypeOf<OkResult>());
    }

    [Test]
    public async Task ServiceDiagnose_HasUnHealthyResult_ReturnsServiceUnavailableStatus()
    {
      // arrange
      var healthResult = new[] {Given.UnHealthyDiagnosticInfo};
      var healthCheckingService = Given.HealthCheckingService.WithCheckResult(healthResult).Build();

      // act
      var controller = new DiagnosticController(healthCheckingService, new NullLogger<DiagnosticController>());
      var result = await controller.ServiceDiagnose();

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.TypeOf<StatusCodeResult>());
        Assert.That((result as StatusCodeResult).StatusCode, Is.EqualTo((int)HttpStatusCode.ServiceUnavailable));
      });
    }

    [Test]
    public async Task Diagnose_HasHealthyResult_ReturnsOkStatus()
    {
      // arrange
      var healthResult = new[] {Given.HealthyDiagnosticInfo};
      var healthCheckingService = Given.HealthCheckingService.WithCheckResult(healthResult).Build();

      // act
      var controller = new DiagnosticController(healthCheckingService, new NullLogger<DiagnosticController>());
      IConvertToActionResult result = await controller.Diagnose();

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.TypeOf<ActionResult<DiagnoseResult>>());
        Assert.That((result.Convert() as ObjectResult).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
      });
    }

    [Test]
    public async Task Diagnose_HasUnHealthyResult_ReturnsServiceUnavailableStatus()
    {
      // arrange
      var healthResult = new[] {Given.UnHealthyDiagnosticInfo};
      var healthCheckingService = Given.HealthCheckingService.WithCheckResult(healthResult).Build();

      // act
      var controller = new DiagnosticController(healthCheckingService, new NullLogger<DiagnosticController>());
      IConvertToActionResult result = await controller.Diagnose();

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.TypeOf<ActionResult<DiagnoseResult>>());
        Assert.That((result.Convert() as ObjectResult).StatusCode, Is.EqualTo((int)HttpStatusCode.ServiceUnavailable));
      });
    }

    [Test]
    public async Task Diagnose_HasUnHealthyResultWIthSkipFlag_ReturnsServiceOkStatus()
    {
      // arrange
      var healthResult = new[] {Given.UnHealthyDiagnosticInfoWIthSkipFlag};
      var healthCheckingService = Given.HealthCheckingService.WithCheckResult(healthResult).Build();

      // act
      var controller = new DiagnosticController(healthCheckingService, new NullLogger<DiagnosticController>());
      IConvertToActionResult result = await controller.Diagnose();

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.TypeOf<ActionResult<DiagnoseResult>>());
        Assert.That((result.Convert() as ObjectResult).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
      });
    }
  }
}