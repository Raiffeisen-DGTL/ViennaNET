using System.Diagnostics;
using System.Linq;
using Company.WebApi.Core.DefaultConfiguration.Diagnostic;
using Company.WebApi.Core.DefaultConfiguration.Https;
using Company.WebApi.Core.DefaultConfiguration.Logging;
using Company.WebApi.Core.DefaultConfiguration.Middleware;
using Company.WebApi.Core.DefaultConfiguration.SimpleInjector;
using Company.WebApi.Core.DefaultHttpSysRunner.HttpClients;
using Company.WebApi.Core.DefaultHttpSysRunner.Security.Ntlm;
using Company.WebApi.Core.DefaultHttpSysRunner.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;

namespace Company.WebApi.Core.DefaultHttpSysRunner
{
  public static class DefaultHttpSysRunner
  {
    public static HostBuilder Configure()
    {
      return HostBuilder.Create()
                        .UseServer(HttpSysConfigurator.Configure)
                        .AddMvcBuilderConfiguration(HttpSysConfigurator.ConfigureMvcBuilder)
                        .ConfigureApp(HttpsConfigurator.Configure)
                        .RegisterServices(HttpsConfigurator.ConfigureRedirect)
                        .CreateContainer(SimpleInjectorConfigurator.CreateContainer)
                        .VerifyContainer(SimpleInjectorConfigurator.VerifyContainer)
                        .ConfigureContainer(SimpleInjectorConfigurator.Configure)
                        .InitializeContainer(SimpleInjectorConfigurator.Initialize)
                        .ConfigureApp(CompanyLoggingConfigurator.Configure, true)
                        .ConfigureApp(CustomMiddlewareConfigurator.Configure)
                        .AddMvcBuilderConfiguration(CompanyHealthCheckingConfigurator.ConfigureMvcBuilder)
                        .RegisterServices(NtlmSecurityConfigurator.Register)
                        .RegisterServices(NtlmHttpClientsConfigurator.RegisterHttpClients);
    }

    /// <summary>
    /// Собирает и запускает сервис либо как консольное приложение, либо как Windows-сервис
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    public static void BuildAndRun(this HostBuilder builder, string[] args)
    {
      var webHost = builder.BuildWebHost(args);

      var isService = !(Debugger.IsAttached || args.Contains("--console"));

      if (isService)
      {
        webHost.RunAsService();
        return;
      }

      webHost.Run();
    }
  }
}
