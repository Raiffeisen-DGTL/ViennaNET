using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValidationService.Validation;
using ValidationService.Validation.Rules;
using ValidationService.Validation.Rulesets;
using ViennaNET.Validation.Validators;
using ViennaNET.WebApi;
using ViennaNET.WebApi.Configurators.Kestrel;
using ViennaNET.WebApi.Configurators.Swagger;

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
                        .RegisterServices(RegisterServices)
                        .BuildWebHost(args)
                        .Run();
    }

    private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
      services.AddSingleton<IValidator, Validator>();

      services.AddSingleton<LengthRule>();
      services.AddSingleton<StartsWithRule>();

      services.AddSingleton<GreetingsRuleset>();

      services.AddSingleton<IGreetingsValidationService, GreetingsValidationService>();
    }
  }
}

