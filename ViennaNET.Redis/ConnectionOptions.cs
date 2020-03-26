using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace ViennaNET.Redis
{
  /// <summary>
  /// Параметры подключения
  /// </summary>
  public class ConnectionOptions
  {
    private readonly ConfigurationOptions _configurationOptions;
    private readonly int _expirationMaxValue;
    private readonly int _expirationMinValue;

    /// <summary>
    /// Инициализирует экземпляр параметрами подключения
    /// </summary>
    /// <param name="configurationOptions">Опции конфигурации</param>
    /// <param name="key">Пользовательский ключ БД</param>
    /// <param name="expirationMinValue">Минимальное значение времени между переподключениями к Redis в случае ошибки</param>
    /// <param name="expirationMaxValue">Максимальное значение времени между переподключениями к Redis в случае ошибки</param>
    /// <param name="keyLifetimes">Коллекция TTL ключей</param>
    public ConnectionOptions(
      ConfigurationOptions configurationOptions, string key, int expirationMinValue, int expirationMaxValue,
      IDictionary<string, TimeSpan> keyLifetimes)
    {
      _configurationOptions = configurationOptions;
      Key = key;
      _expirationMinValue = expirationMinValue;
      _expirationMaxValue = expirationMaxValue;
      KeyLifetimes = keyLifetimes;
    }

    /// <summary>
    /// Пользовательский ключ БД
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Коллекция TTL ключей
    /// </summary>
    public IDictionary<string, TimeSpan> KeyLifetimes { get; }

    /// <summary>
    /// Получает политику переподключения к БД и дозаполняет опции конфигурации 
    /// </summary>
    /// <returns>Опции конфигурации</returns>
    public ConfigurationOptions GetConfigurationOptions()
    {
      _configurationOptions.ReconnectRetryPolicy = new ExponentialRetry(_expirationMinValue, _expirationMaxValue);
      return _configurationOptions;
    }
  }
}
