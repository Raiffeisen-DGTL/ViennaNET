using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ViennaNET.Security;
using ViennaNET.Utils;
using ViennaNET.CallContext;
using ViennaNET.Security.Jwt;
using ViennaNET.WebApi.Net;

namespace ViennaNET.WebApi.Configurators.Security.Jwt
{
  /// <summary>
  /// Фабрика по получению авторизационных данных пользователя из JWT
  /// </summary>
  public class JwtSecurityContextFactory : ISecurityContextFactory
  {
    private readonly ICallContextFactory _callContextFactory;
    private readonly ILoopbackIpFilter _loopbackIpFilter;
    private readonly IJwtTokenReader _jwtTokenReader;

    public JwtSecurityContextFactory(
      ICallContextFactory callContextFactory, ILoopbackIpFilter loopbackIpFilter, IJwtTokenReader jwtTokenReader)
    {
      _callContextFactory = callContextFactory.ThrowIfNull(nameof(callContextFactory));
      _loopbackIpFilter = loopbackIpFilter.ThrowIfNull(nameof(loopbackIpFilter));
      _jwtTokenReader = jwtTokenReader.ThrowIfNull(nameof(jwtTokenReader));
    }

    /// <summary>
    /// Создает контекст и заполняет его из JWT-токена
    /// и заголовков запроса
    /// </summary>
    /// <returns>Контекст с данными пользователя</returns>
    public ISecurityContext Create()
    {
      var callContext = _callContextFactory.Create();

      var permissions = GetPermissions(callContext);
      var ip = _loopbackIpFilter.FilterIp(callContext.RequestCallerIp);

      return new SecurityContext(callContext.UserId, ip, permissions);
    }

    private string[] GetPermissions(ICallContext callContext)
    {
      var callContextPrincipal = _jwtTokenReader.Read(callContext.AuthorizeInfo);
      if (callContextPrincipal != null)
      {
        return ExtractPermissions(callContextPrincipal.Claims);
      }

      return new string[0];
    }

    private string[] ExtractPermissions(IEnumerable<Claim> claims)
    {
      return claims.Where(x => x.Type == ClaimTypes.Role)
                   .Select(x => x.Value)
                   .ToArray();
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
