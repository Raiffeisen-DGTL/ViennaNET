using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.CallContext.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using ViennaNET.CallContext;

namespace ViennaNET.WebApi.Configurators.CallContext
{
  /// <summary>
  /// Включает базовые сервисы и middleware
  /// </summary>
  public static class CallContextConfigurator
  {
    /// <summary>
    /// Включает базовые сервисы и middleware
    /// Необходимо подключать сразу после конфигурирования авторизации
    /// </summary>
    /// <param name="companyHostBuilder"></param>
    /// <returns></returns>
    public static ICompanyHostBuilder UseCallContext(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.ConfigureApp(ConfigureCallContextMiddleware)
                               .RegisterServices(RegisterCallContextServices);
    }

    /// <summary>
    /// Регистрирует сервисы для работы с контекстами
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    internal static void RegisterCallContextServices(IServiceCollection services, object container, IConfiguration configuration)
    {
      var typedContainer = (Container)container;

      typedContainer.RegisterSingleton<IHttpCallContextAccessor, HttpCallContextAccessor>();
      typedContainer.Collection.Append<ICallContextAccessor, HttpCallContextAccessor>(Lifestyle.Singleton);

      typedContainer.RegisterSingleton<ICallContextFactory, CallContextFactory>();

      services.AddSingleton<ICallContextFactory>(x => typedContainer.GetInstance<ICallContextFactory>());
    }

    /// <summary>
    /// Регистрирует базовые middleware в приложении и DI контейнере
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="env"></param>
    /// <param name="container"></param>
    internal static void ConfigureCallContextMiddleware(
      IApplicationBuilder builder, IConfiguration configuration, IHostEnvironment env, object container)
    {
      var typedContainer = (Container)container;
      builder.UseMiddleware<CallContextMiddleware>(typedContainer);
    }
  }
}
