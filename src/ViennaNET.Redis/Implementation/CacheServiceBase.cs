using System;
using System.Text;
using ViennaNET.Utils;

namespace ViennaNET.Redis.Implementation
{
  public abstract class CacheServiceBase
  {
    protected Lazy<IRedisDatabase> _redis;

    public CacheServiceBase(IRedisDatabaseProvider redisDatabaseProvider)
    {
      redisDatabaseProvider.ThrowIfNull(nameof(redisDatabaseProvider));

      _redis = new Lazy<IRedisDatabase>(() =>
      {
        return redisDatabaseProvider.GetDatabase();
      });
    }

    protected static void ThrowIfNoneKeyIdentifier(params object[] keyIdentifier)
    {
      if (keyIdentifier.Length > 0)
      {
        return;
      }
      throw new ArgumentException("Key identifier is empty.", nameof(keyIdentifier));
    }

    protected static string GetRedisKey(string name, params object[] keyIdentifier)
    {
      const char separator = '-';
      var sb = new StringBuilder($"{name}:");
      foreach (var key in keyIdentifier)
      {
        if (key is bool keyBool)
        {
          sb.Append(keyBool ? "1" : "0");
        }
        else
        {
          sb.Append(key);
        }
        sb.Append(separator);
      }
      return sb.ToString().TrimEnd(separator);
    }
  }
}
