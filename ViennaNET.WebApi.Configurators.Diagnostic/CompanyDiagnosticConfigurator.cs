using ViennaNET.WebApi.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using ViennaNET.Diagnostic;

namespace ViennaNET.WebApi.Configurators.Diagnostic
{
  /// <summary>
  /// Регистрирует контроллер с диагностикой сервисов
  /// </summary>
  public static class CompanyDiagnosticConfigurator
  {
    public static ICompanyHostBuilder UseDiagnosing(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.AddMvcBuilderConfiguration(ConfigureMvcBuilder)
                               .RegisterServices(RegisterServices);
    }

    internal static void ConfigureMvcBuilder(IMvcCoreBuilder builder, IConfiguration configuration)
    {
      builder.AddApplicationPart(typeof(DiagnosticController).Assembly);
    }

    internal static void RegisterServices(IServiceCollection services, object container, IConfiguration configuration)
    {
      var typedContainer = (Container)container;

      typedContainer.Collection.Append<IDiagnosticImplementor, EmptyDiagnosticImplementor>();
      typedContainer.RegisterSingleton<IHealthCheckingService, HealthCheckingService>();
    }
  }
}
