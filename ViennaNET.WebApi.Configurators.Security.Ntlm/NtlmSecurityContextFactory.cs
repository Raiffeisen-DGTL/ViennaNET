using System;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using ViennaNET.Security;
using ViennaNET.Utils;
using ViennaNET.WebApi.Net;
using Microsoft.AspNetCore.Http;
using ViennaNET.CallContext;

namespace ViennaNET.WebApi.Configurators.Security.Ntlm
{
  /// <summary>
  /// Фабрика по получению авторизационных данных пользователя из JWT
  /// </summary>
  public class NtlmSecurityContextFactory : ISecurityContextFactory
  {
    private static readonly char[] loginSeparators = { '\\' };

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICallContextFactory _callContextFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILoopbackIpFilter _loopbackIpFilter;

    public NtlmSecurityContextFactory(IHttpContextAccessor httpContextAccessor, 
                                      IHttpClientFactory httpClientFactory,
                                      ILoopbackIpFilter loopbackIpFilter,
                                      ICallContextFactory callContextFactory)
    {
      _httpContextAccessor = httpContextAccessor.ThrowIfNull(nameof(httpContextAccessor));
      _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
      _loopbackIpFilter = loopbackIpFilter.ThrowIfNull(nameof(loopbackIpFilter));
      _callContextFactory = callContextFactory.ThrowIfNull(nameof(callContextFactory));
    }

    /// <summary>
    /// Создает контекст и заполняет его из WindowsIdentity
    /// </summary>
    /// <returns>Контекст</returns>
    public ISecurityContext Create()
    {
      var callContext = _callContextFactory.Create();
      var httpContext = _httpContextAccessor.HttpContext;
      if (!(httpContext?.User?.Identity is WindowsIdentity identity) || !identity.IsAuthenticated)
      {
        identity = WindowsIdentity.GetCurrent();
      }

      var name = string.IsNullOrWhiteSpace(callContext.UserId)
        ? callContext.UserId
        : identity.Name;

      var ip = _loopbackIpFilter.FilterIp(callContext.RequestCallerIp);

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
