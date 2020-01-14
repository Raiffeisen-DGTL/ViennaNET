namespace Company.Security.Jwt
{
  /// <summary>
  /// Интерфейс для доступа к параметрам ключа шифрования
  /// </summary>
  public interface ISecurityKeysContainer
  {
    IJwtSigningEncodingKey GetEncodingKey();
    string Issuer();
    string Audience();
  }
}
