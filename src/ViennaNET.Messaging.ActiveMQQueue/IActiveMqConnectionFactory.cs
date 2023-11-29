using Apache.NMS;

namespace ViennaNET.Messaging.ActiveMQQueue
{
  /// <summary>
  ///   Фабрика подключений к очереди
  /// </summary>
  public interface IActiveMqConnectionFactory
  {
    /// <summary>
    ///   Получить фабрику подключений к очереди
    /// </summary>
    /// <param name="configuration">Настройки подключения</param>
    /// <returns></returns>
    IConnectionFactory GetConnectionFactory(ActiveMqQueueConfiguration configuration);
  }
}