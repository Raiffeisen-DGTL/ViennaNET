using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Company.Security.Jwt.Impl
{
  public class JwtTokenFactory : IJwtTokenFactory
  {
    private readonly ISecurityKeysContainer _securityKeysContainer;

    public JwtTokenFactory(ISecurityKeysContainer securityKeysContainer)
    {
      _securityKeysContainer = securityKeysContainer;
    }


    /// <inheritdoc />
    public string Create(string userName, string[] permissions)
    {
      return Create(userName, permissions, null);
    }

    /// <inheritdoc />
    public string Create(string userName, string[] permissions, IDictionary<string, object> additionalData)
    {
      var claims = new List<Claim>(permissions.Length);
      var signingEncodingKey = _securityKeysContainer.GetEncodingKey();
      var permissionClaims = permissions.Select(p => new Claim(ClaimTypes.Role, p));

      claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));
      claims.AddRange(permissionClaims);

      var token = new JwtSecurityToken(issuer: _securityKeysContainer.Issuer(),
        audience: _securityKeysContainer.Audience(),
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: new SigningCredentials(signingEncodingKey.GetKey(),
        signingEncodingKey.SigningAlgorithm));

      if (additionalData != null)
      {
        foreach (var kvp in additionalData)
        {
          token.Payload.Add(kvp.Key, kvp.Value);
        }
      }

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
