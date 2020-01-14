using System;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Company.Security;
using Company.Utils;
using Company.WebApi.Core.DefaultConfiguration.HttpClients;
using Company.WebApi.Core.Net;

namespace Company.WebApi.Core.DefaultHttpSysRunner.Security.Ntlm
{
  /// <summary>
  /// Фабрика по получению авторизационных данных пользователя из JWT
  /// </summary>
  public class NtlmSecurityContextFactory : ISecurityContextFactory
  {
    private static readonly char[] loginSeparators = { '\\' };

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILoopbackIpFilter _loopbackIpFilter;

    public NtlmSecurityContextFactory(IHttpContextAccessor httpContextAccessor, 
                                      IHttpClientFactory httpClientFactory,
                                      ILoopbackIpFilter loopbackIpFilter)
    {
      _httpContextAccessor = httpContextAccessor.ThrowIfNull(nameof(httpContextAccessor));
      _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
      _loopbackIpFilter = loopbackIpFilter.ThrowIfNull(nameof(loopbackIpFilter));
    }

    /// <summary>
    /// Создает контекст и заполняет его из WindowsIdentity
    /// </summary>
    /// <returns>Контекст</returns>
    public ISecurityContext Create()
    {
      var context = _httpContextAccessor.HttpContext;
      var identity = context?.User?.Identity as WindowsIdentity;
      if (identity is null || !identity.IsAuthenticated)
      {
        identity = WindowsIdentity.GetCurrent();
      }

      var request = context?.Request;

      var name = request != null && request.Headers.Keys.Contains(CompanyHeaders.UserId)
        ? request.Headers[CompanyHeaders.UserId]
                 .ToString()
        : identity.Name;

      var ip = request != null && request.Headers.Keys.Contains(CompanyHeaders.RequestHeaderCallerIp)
        ? context.Request.Headers[CompanyHeaders.RequestHeaderCallerIp]
                  .ToString()
        : context?.Connection?.RemoteIpAddress?.MapToIPv4()
                  .ToString();

      ip = _loopbackIpFilter.FilterIp(ip);

      return new NtlmSecurityContext(CleanUserName(name), ip, _httpClientFactory);
    }

    /// <summary>
    /// Создает контекст и заполняет его из JWT-токена
    /// </summary>
    /// <returns>Контекст</returns>
    public Task<ISecurityContext> CreateAsync()
    {
      return Task.FromResult(Create());
    }

    private string CleanUserName(string name)
    {
      var names = name.Split(loginSeparators, StringSplitOptions.RemoveEmptyEntries);
      return names.Length > 1
        ? names.Last()
        : name;
    }
  }
}
