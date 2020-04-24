using System;

namespace ViennaNET.Messaging.Configuration
{
  /// <summary>
  /// Тип взаимодействия
  /// </summary>
  [Serializable]
  public enum MessageProcessingType
  {
    /// <summary>
    /// Цикличное выполнение
    /// </summary>
    ThreadStrategy = 0,

    /// <summary>
    /// Подписчик
    /// </summary>
    Subscribe = 1,

    /// <summary>
    /// Подписчик с ответом
    /// </summary>
    SubscribeAndReply = 2
  }
}
