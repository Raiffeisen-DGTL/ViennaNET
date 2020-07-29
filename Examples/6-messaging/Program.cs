using MessagingService.Features.Receive;
using Microsoft.AspNetCore.Hosting;
using ViennaNET.WebApi;
using ViennaNET.WebApi.Configurators.CallContext;
using ViennaNET.WebApi.Configurators.Common;
using ViennaNET.WebApi.Configurators.Diagnostic;
using ViennaNET.WebApi.Configurators.Kestrel;
using ViennaNET.WebApi.Configurators.SimpleInjector;
using ViennaNET.WebApi.Configurators.Swagger;
using ViennaNET.WebApi.Runners.Extensions;

namespace MessagingService
{
  internal static class Program
  {
    internal static void Main(string[] args)
    {
      CompanyHostBuilder.Create()
                        .UseCommonModules()
                        .UseKestrel()
                        .UseSwagger()
                        .UseSimpleInjector()
                        .UseCallContext()
                        .UseDiagnosing()
                        .AddOnStartAction<Listener>(x => x.StartListening())
                        .AddOnStopAction<Listener>(x => x.StopListening())
                        .BuildWebHost(args)
                        .Run();
    }
  }
}
