using Company.WebApi.Core.DefaultConfiguration.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.WebApi.Core.DefaultHttpSysRunner.Service
{
  public static class HttpSysConfigurator
  {
    /// <summary>
    /// Добавляет обязательность авторизации ко всем запросам
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    public static void ConfigureMvcBuilder(IMvcCoreBuilder builder, IConfiguration configuration)
    {
      var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .Build();
      builder.AddAuthorization()
             .AddMvcOptions(o => o.Filters.Add(new AuthorizeFilter(policy)));
    }

    /// <summary>
    /// Регистрирует HttpSys как сервер
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    public static void Configure(IWebHostBuilder builder)
    {
      builder.UseHttpSys()
             .ConfigureServices((context, services) =>
             {
               services.Configure<HttpSysOptions>(options => SetHttpSysOptions(context, options));
             });
    }

    private static void SetHttpSysOptions(WebHostBuilderContext context, HttpSysOptions options)
    {
      var hostConfiguration = context.Configuration.GetSection(CompanyWebApiConfiguration.SectionName)
                                     .Get<CompanyWebApiConfiguration>();
      options.Authentication.Schemes = AuthenticationSchemes.NTLM | AuthenticationSchemes.Negotiate;
      options.Authentication.AllowAnonymous = true;

      options.UrlPrefixes.Add($"http://*:{hostConfiguration.PortNumber}");

      if (hostConfiguration.HttpsPort.HasValue)
      {
        options.UrlPrefixes.Add($"https://*:{hostConfiguration.HttpsPort}");
      }
    }
  }
}
