using System;

namespace ViennaNET.Redis.Configuration
{
  [Serializable]
  internal class RedisKeyLifetimeElement
  {
    public string Name { get; set; }

    public TimeSpan Time { get; set; }

    public override string ToString()
    {
      return $"{Name}:{Time}";
    }
  }
}