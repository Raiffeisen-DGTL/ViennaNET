using System;
using System.Collections.Generic;
using System.Linq;
using ViennaNET.Messaging.Exceptions;

namespace ViennaNET.Messaging.Configuration
{
  /// <summary>
  ///   Конфигурация для работы с очередями
  /// </summary>
  public abstract class ConfigurationsListBase<T> where T : QueueConfigurationBase
  {
    /// <summary>
    ///   Коллекция конфигураций очередей <see cref="QueueConfigurationBase" />
    /// </summary>
    public List<T> Queues { get; set; } = new List<T>();

    /// <summary>
    ///   Возвращает конфигурацию очереди по переданному идентификатору
    /// </summary>
    /// <param name="queueId">Идентификатор очереди</param>
    /// <returns>Конфигурация очереди</returns>
    public T GetQueueConfiguration(string queueId)
    {
      T queueConfiguration;
      try
      {
        queueConfiguration = Queues.SingleOrDefault(q => q.Id == queueId);
      }
      catch (Exception e)
      {
        throw new MessagingConfigurationException(e, $"There are too many configuration with id '{queueId}' in configuration file");
      }

      return queueConfiguration;
    }
  }
}