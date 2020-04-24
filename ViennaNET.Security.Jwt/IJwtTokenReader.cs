using System.Security.Claims;

namespace ViennaNET.Security.Jwt
{
  /// <summary>
  /// Сервис для расшифровки токена
  /// </summary>
  public interface IJwtTokenReader
  {
    /// <summary>
    /// Расшифровывает токен
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ClaimsPrincipal Read(string token);
  }
}
