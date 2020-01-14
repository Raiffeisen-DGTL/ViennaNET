namespace Company.WebApi.Core.DefaultConfiguration.Security.Jwt
{
  public class JwtSecurityConfiguration
  {
    /// <summary>
    /// Название секции в кофнигурационном файле
    /// </summary>
    public const string SectionName = "jwtSecuritySettings";

    /// <summary>
    /// Издатель
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// Аудитория
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Название переменной окружения, хранящей ключ шифрования токена
    /// </summary>
    public string TokenKeyEnvVariable { get; set; }

    /// <summary>
    /// Алгоритм шифрования
    /// </summary>
    public string SigningAlgorithm { get; set; }
  }
}