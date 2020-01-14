using Company.Security;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.WebApi.Core.DefaultHttpSysRunner.Security.Ntlm
{
  /// <summary>
  /// Настраивает NTLM-авторизацию
  /// </summary>
  public static class NtlmSecurityConfigurator
  {
    /// <summary>
    /// Добавляет NTLM-авторизацию
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
