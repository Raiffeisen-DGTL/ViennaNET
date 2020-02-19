using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ViennaNET.Logging;
using ViennaNET.Security;
using ViennaNET.Utils;
using ViennaNET.WebApi.Net;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Configurators.Security.Jwt
{
  /// <summary>
  /// Фабрика по получению авторизационных данных пользователя из JWT
  /// </summary>
  public class JwtSecurityContextFactory : ISecurityContextFactory
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILoopbackIpFilter _loopbackIpFilter;

    public JwtSecurityContextFactory(IHttpContextAccessor httpContextAccessor,
                                     ILoopbackIpFilter loopbackIpFilter)
    {
      _httpContextAccessor = httpContextAccessor.ThrowIfNull(nameof(httpContextAccessor));
      _loopbackIpFilter = loopbackIpFilter.ThrowIfNull(nameof(loopbackIpFilter));
    }

    /// <summary>
    /// Создает контекст и заполняет его из JWT-токена
    /// и заголовков запроса
    /// </summary>
    /// <returns>Контекст с данными пользователя</returns>
    public ISecurityContext Create()
    {
      var context = _httpContextAccessor.HttpContext;
      var identity = context?.User?.Identity;
      if (identity is null || !identity.IsAuthenticated)
      {
        // NOTE: Данная ситуация возможна только при наличии AllowAnonymous-аттрибута
        Logger.LogWarning("Invalid token");
        identity = null;
      }

      var username = string.Empty;
      var permissions = new string[0];

      if (identity != null)
      {
        username = context.User?.Claims
                          ?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                          ?.Value;

        var contextPermissions = context.User?.Claims
                                              ?.Where(x => x.Type == ClaimTypes.Role)
                                              .Select(x => x.Value)
                                              .ToArray();
        if (contextPermissions != null)
        {
          permissions = contextPermissions;
        }
      }

      var request = context?.Request;

      if (string.IsNullOrEmpty(username))
      {
        username = request != null && request.Headers.Keys.Contains(CompanyHttpHeaders.UserId)
          ? request.Headers[CompanyHttpHeaders.UserId]
                   .ToString()
          : Environment.UserName;
      }

      var ip = request != null && request.Headers.Keys.Contains(CompanyHttpHeaders.RequestHeaderCallerIp)
        ? context.Request.Headers[CompanyHttpHeaders.RequestHeaderCallerIp]
                 .ToString()
        : context?.Connection?.RemoteIpAddress?.MapToIPv4()
                 .ToString();

      ip = _loopbackIpFilter.FilterIp(ip);

      return new SecurityContext(username, ip, permissions);
    }

    /// <summary>
    /// Создает контекст и заполняет его из JWT-токена
    /// </summary>
    /// <returns>Контекст с данными пользователя</returns>
    public Task<ISecurityContext> CreateAsync()
    {
      return Task.FromResult(Create());
    }
  }
}
