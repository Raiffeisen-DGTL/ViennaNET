using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SimpleInjector;

namespace Company.WebApi.Core.DefaultConfiguration.Middleware
{
  /// <summary>
  /// Конфигуратор для регистрации кастомных middleware в приложении и DI контейнере (SimpleInjector)
  /// </summary>
  public static class CustomMiddlewareConfigurator
  {
    public static void Configure(IApplicationBuilder builder, IConfiguration configuration, IHostingEnvironment env, object container)
    {
      var typedContainer = container as Container;

      builder.UseMiddleware<RequestRegistrationMiddleware>(typedContainer);
      builder.UseMiddleware<SetUpLoggerMiddleware>(typedContainer);
      builder.UseMiddleware<LogRequestAndResponseMiddleware>(typedContainer);
    }
  }
}
