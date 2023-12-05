using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using ViennaNET.Redis.Exceptions;

namespace ViennaNET.Redis.Utils
{
  internal static class CompressUtils
  {
    public static string CompressString(string value)
    {
      try
      {
        var bytes = Encoding.Default.GetBytes(value);
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
          using (var gs = new GZipStream(mso, CompressionMode.Compress))
          {
            msi.CopyTo(gs);
          }

          return Convert.ToBase64String(mso.ToArray());
        }
      }
      catch (Exception e)
      {
        throw new RedisException("Error during compression.", e);
      }
    }

    public static string DecompressString(string value)
    {
      if (value == null)
      {
        return null;
      }

      try
      {
        var bytes = Convert.FromBase64String(value);
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
          using (var gs = new GZipStream(msi, CompressionMode.Decompress))
          {
            gs.CopyTo(mso);
          }

          return Encoding.Default.GetString(mso.ToArray());
        }
      }
      catch (Exception e)
      {
        throw new RedisException("Error during decompression.", e);
      }
    }
  }
}