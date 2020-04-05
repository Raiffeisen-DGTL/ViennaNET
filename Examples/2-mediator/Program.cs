using Microsoft.AspNetCore.Hosting;
using ViennaNET.WebApi;
using ViennaNET.WebApi.Configurators.Kestrel;
using ViennaNET.WebApi.Configurators.Swagger;
using ViennaNET.WebApi.Configurators.SimpleInjector;

namespace MediatorService
{
  class Program
  {
    static void Main(string[] args)
    {
      CompanyHostBuilder.Create()
                        .UseDefaultLogging()
                        .UseKestrel()
                        .UseSwagger()
                        .UseSimpleInjector()
                        .BuildWebHost(args)
                        .Run();
    }
  }
}
