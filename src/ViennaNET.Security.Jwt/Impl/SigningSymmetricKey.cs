using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ViennaNET.Security.Jwt.Impl
{
  public class SigningSymmetricKey : IJwtSigningEncodingKey, IJwtSigningDecodingKey
  {
    private readonly SymmetricSecurityKey _secretKey;

    public SigningSymmetricKey(string key, string signingAlgorithm)
    {
      _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
      SigningAlgorithm = signingAlgorithm;
    }

    public string SigningAlgorithm { get; }

    public SecurityKey GetKey()
    {
      return _secretKey;
    }
  }
}