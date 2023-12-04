using System;
using System.Collections.Generic;

namespace ViennaNET.Redis.Configuration
{
  [Serializable]
  internal class RedisConfiguration
  {
    private const int ExpirationMin = 5000;
    private const int ExpirationMax = 20000;
    private int _expirationMaxValue;

    private int _expirationMinValue;

    public RedisConfiguration()
    {
      KeyLifetimes = new List<RedisKeyLifetimeElement>();
    }

    public List<RedisKeyLifetimeElement> KeyLifetimes { get; }

    public string Connection { get; set; }

    public string Key { get; set; }

    public int ExpirationMinValue
    {
      get => _expirationMinValue;
      set =>
        _expirationMinValue = value <= 0
          ? ExpirationMin
          : value;
    }

    public int ExpirationMaxValue
    {
      get => _expirationMaxValue;
      set =>
        _expirationMaxValue = value <= 0
          ? ExpirationMax
          : value;
    }

    public bool ClearOnStartup { get; set; } = true;
  }
}