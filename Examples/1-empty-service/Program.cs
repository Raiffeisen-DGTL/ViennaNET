using Microsoft.AspNetCore.Hosting;
using ViennaNET.WebApi;
using ViennaNET.WebApi.Configurators.Kestrel;
using ViennaNET.WebApi.Configurators.Swagger;

namespace EmptyService
{
  class Program
  {
    static void Main(string[] args)
    {
      CompanyHostBuilder.Create()
                        .UseDefaultLogging()
                        .UseKestrel()
                        .UseSwagger()
                        .BuildWebHost(args)
                        .Run();
    }
  }
}
