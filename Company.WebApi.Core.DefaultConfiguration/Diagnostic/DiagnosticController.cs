using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Company.Diagnostic.Core;
using Company.Diagnostic.Core.Data;
using Company.Utils;
using Company.WebApi.Core.DefaultConfiguration.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Company.WebApi.Core.DefaultConfiguration.Diagnostic
{
  /// <summary>
  /// Контроллер для проведения диагностики сервиса
  /// </summary>
  [Route("diagnostic")]
  public class DiagnosticController : ControllerBase
  {
    private readonly IHealthCheckingService _healthCheckingService;
    private readonly IConfiguration _configuration;

    public DiagnosticController(IHealthCheckingService healthCheckingService, IConfiguration configuration)
    {
      _healthCheckingService = healthCheckingService.ThrowIfNull(nameof(healthCheckingService));
      _configuration = configuration.ThrowIfNull(nameof(configuration));
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

      var hostConfiguration = _configuration.GetSection(CompanyWebApiConfiguration.SectionName)
                                            .Get<CompanyWebApiConfiguration>();
      return new DiagnoseResult
      {
        Name = service.ToString(),
        Host = $"{Dns.GetHostName()}:{hostConfiguration.PortNumber}",
        Version = service.Version.ToString(),
        HasErrors = healthCheckResults.Any(x => x.Status != DiagnosticStatus.Ok && !x.IsSkipResult),
        Results = healthCheckResults.Select(x => new EndpointResult()
        {
          Name = x.Name,
          Url = x.Url,
          Status = x.Status.ToString(),
          Error = x.Error
        })
      };
    }
  }
}
