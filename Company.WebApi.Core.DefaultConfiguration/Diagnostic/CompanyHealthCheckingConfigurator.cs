using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.WebApi.Core.DefaultConfiguration.Diagnostic
{
  /// <summary>
  /// Регистрирует контроллер с диагностикой сервисов
  /// </summary>
  public static class CompanyHealthCheckingConfigurator
  {
    public static void ConfigureMvcBuilder(IMvcCoreBuilder builder, IConfiguration configuration)
    {
      builder.AddApplicationPart(typeof(DiagnosticController).Assembly);
    }
  }
}
