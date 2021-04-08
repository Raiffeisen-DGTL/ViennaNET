using Apache.NMS;

namespace ViennaNET.Messaging.ActiveMQQueue
{
  /// <summary>
  /// Фабрика подключений к очереди
  /// </summary>
  public interface IActiveMqConnectionFactory
  {
    /// <summary>
    /// Получить фабрику подключений к очереди
    /// </summary>
    /// <param name="server">Сервер</param>
    /// <param name="port">Порт подключения</param>
    /// <returns></returns>
    IConnectionFactory GetConnectionFactory(string server, int port);
  }
}
