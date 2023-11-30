using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ViennaNET.Security.Jwt.Impl
{
  public class JwtTokenReader : IJwtTokenReader
  {
    private readonly ILogger _logger;
    private readonly ISecurityKeysContainer _securityKeysContainer;

    public JwtTokenReader(ISecurityKeysContainer securityKeysContainer, ILogger<JwtTokenReader> logger)
    {
      _securityKeysContainer = securityKeysContainer;
      _logger = logger;
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
        _logger.LogError(ex, ex.Message);
        _logger.LogTrace("JwtTokenReader: cannot read token {Token}", token);
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
      return new()
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