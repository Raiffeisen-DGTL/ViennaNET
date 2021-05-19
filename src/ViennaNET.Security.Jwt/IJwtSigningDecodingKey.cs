using Microsoft.IdentityModel.Tokens;

namespace ViennaNET.Security.Jwt
{
  /// <summary>
  /// Ключ для проверки подписи (публичный)
  /// </summary>
  public interface IJwtSigningDecodingKey
  {
    SecurityKey GetKey();
  }
}
