using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.HttpSys.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ViennaNET.WebApi.Configurators.HttpSys
{
  public static class HttpSysConfigurator
  {
    /// <summary>
    /// Регистрирует Kestrel как сервер на основе параметров в конфигурации
    /// </summary>
    public static ICompanyHostBuilder UseHttpSys(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.UseServer(ConfigureHttpSys)
                               .ConfigureApp(ConfigureHsts)
                               .AddMvcBuilderConfiguration(ConfigureRequiredAuth);
    }

    /// <summary>
    /// Добавляет обязательность авторизации ко всем запросам
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    internal static void ConfigureRequiredAuth(IMvcCoreBuilder builder, IConfiguration configuration)
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
    internal static void ConfigureHttpSys(IWebHostBuilder builder)
    {
      builder.UseHttpSys()
             .ConfigureServices((context, services) =>
             {
               services.Configure<HttpSysOptions>(options => SetHttpSysOptions(context, options));
             });
    }

    internal static void SetHttpSysOptions(WebHostBuilderContext context, HttpSysOptions options)
    {
      var hostConfiguration = context.Configuration.GetSection(WebApiConfiguration.SectionName)
                                     .Get<WebApiConfiguration>();
      options.Authentication.Schemes = AuthenticationSchemes.NTLM | AuthenticationSchemes.Negotiate;
      options.Authentication.AllowAnonymous = true;

      options.UrlPrefixes.Add($"http://*:{hostConfiguration.PortNumber}");

      if (hostConfiguration.HttpsPort.HasValue)
      {
        options.UrlPrefixes.Add($"https://*:{hostConfiguration.HttpsPort}");
      }
    }

    /// <summary>
    /// Настраивает Hsts
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="env"></param>
    /// <param name="container"></param>
    internal static void ConfigureHsts(IApplicationBuilder builder, IConfiguration configuration, IHostEnvironment env, object container)
    {
      var hostConfiguration = configuration.GetSection(WebApiConfiguration.SectionName)
                                           .Get<WebApiConfiguration>();

      if (!hostConfiguration.HttpsPort.HasValue)
      {
        return;
      }

      if (hostConfiguration.UseHsts == true && !env.IsDevelopment())
      {
        builder.UseHsts();
      }

      if (hostConfiguration.UseStrictHttps.HasValue && hostConfiguration.UseStrictHttps.Value)
      {
        builder.UseHttpsRedirection();
      }
    }
  }
}
