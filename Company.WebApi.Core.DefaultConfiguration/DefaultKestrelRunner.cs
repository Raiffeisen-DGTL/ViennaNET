using Company.WebApi.Core.DefaultConfiguration.Diagnostic;
using Company.WebApi.Core.DefaultConfiguration.HttpClients;
using Company.WebApi.Core.DefaultConfiguration.Https;
using Company.WebApi.Core.DefaultConfiguration.Kestrel;
using Company.WebApi.Core.DefaultConfiguration.Logging;
using Company.WebApi.Core.DefaultConfiguration.Middleware;
using Company.WebApi.Core.DefaultConfiguration.Security.Jwt;
using Company.WebApi.Core.DefaultConfiguration.SimpleInjector;
using Company.WebApi.Core.DefaultConfiguration.Swagger;

namespace Company.WebApi.Core.DefaultConfiguration
{
  public static class DefaultKestrelRunner
  {
    public static HostBuilder Configure()
    {
      return HostBuilder.Create()
                        .UseServer(KestrelConfigurator.Configure)
                        .ConfigureApp(HttpsConfigurator.Configure)
                        .RegisterServices(HttpsConfigurator.ConfigureRedirect)
                        .CreateContainer(SimpleInjectorConfigurator.CreateContainer)
                        .VerifyContainer(SimpleInjectorConfigurator.VerifyContainer)
                        .ConfigureContainer(SimpleInjectorConfigurator.Configure)
                        .InitializeContainer(SimpleInjectorConfigurator.Initialize)
                        .ConfigureApp(CompanyLoggingConfigurator.Configure, true)
                        .ConfigureApp(JwtSecurityConfigurator.UseAuthentication)
                        .ConfigureApp(CustomMiddlewareConfigurator.Configure)
                        .AddMvcBuilderConfiguration(JwtSecurityConfigurator.ConfigureMvcBuilder)
                        .AddMvcBuilderConfiguration(CompanyHealthCheckingConfigurator.ConfigureMvcBuilder)
                        .RegisterServices(JwtSecurityConfigurator.Register)
                        .RegisterServices(JwtHttpClientsConfigurator.RegisterHttpClients)
                        .AddSwaggerConfigurations(SwaggerJwtAuthConfigurator.ConfigureSwaggerUiOptions,
                                                  SwaggerJwtAuthConfigurator.ConfigureSwaggerGenOptions);
    }
  }
}
