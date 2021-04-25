using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  /// <summary>
  /// Настройки очереди
  /// </summary>
  public class MqSeriesQueueConfiguration : QueueConfigurationBase
  {
    /// <summary>
    /// Клиентский идентификатор
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    ///   Порт
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// Имя менеджера очередей
    /// </summary>
    public string QueueManager { get; set; }

    /// <summary>
    /// Имя канала
    /// </summary>
    public string Channel { get; set; }

    /// <summary>
    /// Селектор для вычитывания из очереди отфильтрованных сообщений
    /// </summary>
    public string Selector { get; set; }

    /// <summary>
    /// Строка подключения
    /// </summary>
    public string QueueString { get; set; }

    /// <summary>
    /// Признак работы в транзакции
    /// </summary>
    public bool TransactionEnabled { get; set; }

    /// <summary>
    /// Признак использования строки подключения
    /// </summary>
    public bool UseQueueString { get; set; }
  }
}
