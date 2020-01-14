using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Company.Security.Jwt.Impl
{
  public class SigningSymmetricKey : IJwtSigningEncodingKey, IJwtSigningDecodingKey
  {
    private readonly SymmetricSecurityKey _secretKey;

    public string SigningAlgorithm { get; }

    public SigningSymmetricKey(string key, string signingAlgorithm)
    {
      _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
      SigningAlgorithm = signingAlgorithm;
    }

    public SecurityKey GetKey() => _secretKey;
  }
}