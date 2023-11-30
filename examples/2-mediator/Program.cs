using Microsoft.AspNetCore.Hosting;
using ViennaNET.WebApi;
using ViennaNET.WebApi.Configurators.Kestrel;
using ViennaNET.WebApi.Configurators.SimpleInjector;
using ViennaNET.WebApi.Configurators.Swagger;

namespace MediatorService
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      CompanyHostBuilder.Create()
        .UseKestrel()
        .UseSwagger()
        .UseSimpleInjector()
        .BuildWebHost(args)
        .Run();
    }
  }
}