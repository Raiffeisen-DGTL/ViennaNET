using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using ViennaNET.Logging;

namespace ViennaNET.Security.Jwt.Impl
{
  public class JwtTokenReader : IJwtTokenReader
  {
    private readonly ISecurityKeysContainer _securityKeysContainer;

    public JwtTokenReader(ISecurityKeysContainer securityKeysContainer)
    {
      _securityKeysContainer = securityKeysContainer;
    }

    /// <inheritdoc />
    public ClaimsPrincipal Read(string token)
    {
      if (string.IsNullOrWhiteSpace(token))
      {
        return null;
      }

      try
      {
        var handler = new JwtSecurityTokenHandler();
        var clearedToken = ClearToken(token);

        return handler.ValidateToken(clearedToken, GetValidationParameters(_securityKeysContainer), out _);
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, $"JwtTokenReader: cannot read token {token}");
        return null;
      }
    }

    private static string ClearToken(string token)
    {
      return token.Split(' ')
                  .Last();
    }

    private static TokenValidationParameters GetValidationParameters(ISecurityKeysContainer keysContainer)
    {
      return new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = keysContainer.GetEncodingKey()
                                        .GetKey(),
        ValidateIssuer = true,
        ValidIssuer = keysContainer.Issuer(),
        ValidateAudience = true,
        ValidAudience = keysContainer.Audience(),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(5)
      };
    }
  }
}
