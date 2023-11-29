using System;
using MessagingService.Features.Receive;
using Microsoft.Extensions.Hosting;
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
                        .AddOnStoppingAction<Listener>(x => x.StopListening())
                        .AddOnStoppedAction(_ => Console.WriteLine("Done"))
                        .BuildWebHost(args)
                        .Run();
    }
  }
}