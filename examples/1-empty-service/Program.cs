using Microsoft.AspNetCore.Hosting;
using ViennaNET.WebApi;
using ViennaNET.WebApi.Configurators.Kestrel;
using ViennaNET.WebApi.Configurators.Swagger;

namespace EmptyService
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      CompanyHostBuilder.Create()
        .UseKestrel()
        .UseSwagger()
        .BuildWebHost(args)
        .Run();
    }
  }
}