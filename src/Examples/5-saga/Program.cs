using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaService.Sagas;
using SagaService.Services;
using ViennaNET.WebApi;
using ViennaNET.WebApi.Configurators.Kestrel;
using ViennaNET.WebApi.Configurators.Swagger;

namespace SagaService
{
  internal static class Program
  {
    internal static void Main(string[] args)
    {
      ViennaHostBuilder.Create()
                        .UseKestrel()
                        .UseSwagger()
                        .RegisterServices(RegisterServices)
                        .BuildWebHost(args)
                        .Run();
    }

    private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
      services.AddSingleton<TalkSaga>();
      services.AddSingleton<IEnglishService, EnglishService>();
      services.AddSingleton<IDeutschService, DeutschService>();
    }
  }
}
