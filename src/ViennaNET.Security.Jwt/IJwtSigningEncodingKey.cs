using Microsoft.IdentityModel.Tokens;

namespace ViennaNET.Security.Jwt
{
  /// <summary>
  /// Ключ для создания подписи (приватный)
  /// </summary>
  public interface IJwtSigningEncodingKey
  {
    string SigningAlgorithm { get; }
    SecurityKey GetKey();
  }
}
