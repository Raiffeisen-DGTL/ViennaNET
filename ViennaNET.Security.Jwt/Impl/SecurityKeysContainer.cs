using System;
using Microsoft.IdentityModel.Tokens;

namespace ViennaNET.Security.Jwt.Impl
{
  public class SecurityKeysContainer : ISecurityKeysContainer
  {
    private const string defaultEnvironmentVariable = "TOKEN_SECRET_KEY";
    private const string defaultAudience = "TokenAudience";
    private const string defaultIssuer = "TokenIssuer";
    private const string defaultSigningAlgorithm = SecurityAlgorithms.HmacSha256;

    private readonly SigningSymmetricKey _key;
    private readonly string _audience;
    private readonly string _issuer;

    public string Issuer() =>
      string.IsNullOrWhiteSpace(_issuer)
        ? defaultIssuer
        : _issuer;

    public string Audience() =>
      string.IsNullOrWhiteSpace(_audience)
        ? defaultAudience
        : _audience;

    public SecurityKeysContainer() : this(defaultEnvironmentVariable, defaultAudience, defaultIssuer,
      defaultSigningAlgorithm)
    {
    }

    public SecurityKeysContainer(string keyEnvVariable, string audience, string issuer, string signingAlgorithm)
    {
      _audience = audience;
      _issuer = issuer;

      var signingSecurityKey = GetSigningSecurityKey(keyEnvVariable);

      signingAlgorithm = string.IsNullOrWhiteSpace(signingAlgorithm)
        ? defaultSigningAlgorithm
        : signingAlgorithm;

      _key = new SigningSymmetricKey(signingSecurityKey, signingAlgorithm);
    }

    private string GetSigningSecurityKey(string keyEnvVariable)
    {
      var envKey = string.IsNullOrWhiteSpace(keyEnvVariable)
        ? defaultEnvironmentVariable
        : keyEnvVariable;

      var signingSecurityKey = Environment.GetEnvironmentVariable(envKey);

      if (signingSecurityKey is null)
      {
        throw new ArgumentException($"Environment variable {envKey} is not defined");
      }

      if (signingSecurityKey.Length < 16)
      {
        throw new ArgumentOutOfRangeException(
          $"Signing security key from variable {envKey} must be at least 16 symbols length");
      }

      return signingSecurityKey;
    }

    public IJwtSigningDecodingKey GetDecodingKey()
    {
      return _key;
    }

    public IJwtSigningEncodingKey GetEncodingKey()
    {
      return _key;
    }
  }
}