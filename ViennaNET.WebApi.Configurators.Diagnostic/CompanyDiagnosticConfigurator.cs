using ViennaNET.WebApi.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ViennaNET.WebApi.Configurators.Diagnostic
{
  /// <summary>
  /// Регистрирует контроллер с диагностикой сервисов
  /// </summary>
  public static class CompanyDiagnosticConfigurator
  {
    public static ICompanyHostBuilder UseDiagnosing(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.AddMvcBuilderConfiguration(ConfigureMvcBuilder);
    }

    internal static void ConfigureMvcBuilder(IMvcCoreBuilder builder, IConfiguration configuration)
    {
      builder.AddApplicationPart(typeof(DiagnosticController).Assembly);
    }
  }
}
