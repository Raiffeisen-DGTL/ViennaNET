using ViennaNET.Security;
using ViennaNET.WebApi.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ViennaNET.WebApi.Configurators.Security.Ntlm
{
  /// <summary>
  /// Настраивает NTLM-авторизацию
  /// </summary>
  public static class NtlmSecurityConfigurator
  {
    /// <summary>
    /// Добавляет NTLM-авторизацию
    /// </summary>
    /// <param name="companyHostBuilder"></param>
    public static ICompanyHostBuilder UseNtlmAuth(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.ConfigureApp(UseAuthentication)
                               .AddMvcBuilderConfiguration(ConfigureMvcBuilder)
                               .RegisterServices(Register);
    }

    internal static void UseAuthentication(IApplicationBuilder app, IConfiguration configuration, IHostEnvironment env, object container)
    {
      app.UseAuthentication();
    }

    /// <summary>
    /// Добавляет обязательность авторизации ко всем запросам
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    internal static void ConfigureMvcBuilder(IMvcCoreBuilder builder, IConfiguration configuration)
    {
      var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .Build();
      builder.AddAuthorization()
             .AddMvcOptions(o => o.Filters.Add(new AuthorizeFilter(policy)));
    }

    /// <summary>
    /// Регистрирует необходимые сервисы, схемы и атрибуты для NTLM-авторизации
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
      services.AddAuthentication(HttpSysDefaults.AuthenticationScheme);
      services.AddSingleton<ISecurityContextFactory, NtlmSecurityContextFactory>();
      services.AddScoped<WithPermissionsAttribute>();
    }
  }
}
