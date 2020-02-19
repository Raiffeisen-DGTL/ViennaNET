using System;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using ViennaNET.Security;
using ViennaNET.Utils;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Net;
using Microsoft.AspNetCore.Http;

namespace ViennaNET.WebApi.Configurators.Security.Ntlm
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

      var name = request != null && request.Headers.Keys.Contains(CompanyHttpHeaders.UserId)
        ? request.Headers[CompanyHttpHeaders.UserId]
                 .ToString()
        : identity.Name;

      var ip = request != null && request.Headers.Keys.Contains(CompanyHttpHeaders.RequestHeaderCallerIp)
        ? context.Request.Headers[CompanyHttpHeaders.RequestHeaderCallerIp]
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
