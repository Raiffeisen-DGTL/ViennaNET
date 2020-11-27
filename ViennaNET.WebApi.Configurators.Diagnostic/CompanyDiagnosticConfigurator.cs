using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
  [ExcludeFromCodeCoverage]
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
      typedContainer.Collection.Register<IDiagnosticImplementor>(new Type[0]);
      services.AddSingleton<IDiagnosticImplementor, EmptyDiagnosticImplementor>();

      services.AddSingleton<IHealthCheckingService>(p =>
      {
        var internalImplementors = p.GetServices<IDiagnosticImplementor>();
        var externalImplementors = typedContainer.GetAllInstances<IDiagnosticImplementor>();
        
        var allImplementors = new List<IDiagnosticImplementor>();
        allImplementors.AddRange(internalImplementors);
        allImplementors.AddRange(externalImplementors);

        return new HealthCheckingService(allImplementors);
      });
    }
  }
}
