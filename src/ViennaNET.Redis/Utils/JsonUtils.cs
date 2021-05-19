using Newtonsoft.Json;
using ViennaNET.Redis.Exceptions;

namespace ViennaNET.Redis.Utils
{
  internal static class JsonUtils
  {
    public static T DeserializeObject<T>(string value) where T : class
    {
      if (value == null)
      {
        return null;
      }

      try
      {
        return JsonConvert.DeserializeObject<T>(value);
      }
      catch (JsonException e)
      {
        throw new RedisException("Error of object deserialize.", e);
      }
    }

    public static string SerializeObject(object value)
    {
      try
      {
        return JsonConvert.SerializeObject(value);
      }
      catch (JsonException e)
      {
        throw new RedisException("Error of object serialize.", e);
      }
    }
  }
}
