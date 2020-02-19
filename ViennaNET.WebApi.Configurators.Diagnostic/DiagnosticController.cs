using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using ViennaNET.Utils;

namespace ViennaNET.WebApi.Configurators.Diagnostic
{
  /// <summary>
  /// Контроллер для проведения диагностики сервиса
  /// </summary>
  [Route("diagnostic")]
  public class DiagnosticController : ControllerBase
  {
    private const string emptyDiagnosticImplementorName = "empty";
    private readonly IHealthCheckingService _healthCheckingService;

    public DiagnosticController(IHealthCheckingService healthCheckingService)
    {
      _healthCheckingService = healthCheckingService.ThrowIfNull(nameof(healthCheckingService));
    }

    /// <summary>
    /// Пинг сервиса
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("ping")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    public IActionResult Ping()
    {
      return Ok();
    }

    /// <summary>
    /// Диагностика сервиса без авторизации и без выдачи результата в открытую
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("service-diagnose")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    [ProducesResponseType(503)]
    public async Task<IActionResult> ServiceDiagnose()
    {
      var result = await GetDiagnose();

      return result.HasErrors
        ? StatusCode((int)HttpStatusCode.ServiceUnavailable)
        : Ok();
    }

    /// <summary>
    /// Обеспечивает диагностику всех интеграций сервиса
    /// </summary>
    /// <returns>Диагностическую информацию</returns>
    [HttpGet("diagnose")]
    [ProducesResponseType(typeof(DiagnoseResult), 200)]
    [ProducesResponseType(typeof(DiagnoseResult), 503)]
    public async Task<ActionResult<DiagnoseResult>> Diagnose()
    {
      var result = await GetDiagnose();

      return result.HasErrors
        ? StatusCode((int)HttpStatusCode.ServiceUnavailable, result)
        : Ok(result);
    }

    private async Task<DiagnoseResult> GetDiagnose()
    {
      var healthCheckResults = await _healthCheckingService.CheckHealthAsync();

      var service = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName();

      return new DiagnoseResult
      {
        Name = service.ToString(),
        Host = $"{Dns.GetHostName()}",
        Version = service.Version.ToString(),
        HasErrors = healthCheckResults.Any(x => x.Status != DiagnosticStatus.Ok && !x.IsSkipResult),
        Results = healthCheckResults.Where(x => x.Name != emptyDiagnosticImplementorName)
                                    .Select(x => new EndpointResult()
                                    {
                                      Name = x.Name, Url = x.Url, Status = x.Status.ToString(), Error = x.Error
                                    })
      };
    }
  }
}
