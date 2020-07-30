using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  /// <summary>
  ///   Настройки очереди
  /// </summary>
  public class RabbitMqQueueConfiguration : QueueConfigurationBase
  {
    /// <summary>
    ///   Порт
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    ///   Тип обмена (Только для 'RabbitMq')
    /// </summary>
    public string ExchangeType { get; set; }

    /// <summary>
    ///   Имя обмена
    /// </summary>
    public string ExchangeName { get; set; }

    /// <summary>
    ///   Таймаут ответа
    /// </summary>
    public int? ReplyTimeout { get; set; }

    /// <summary>
    ///   Virtual host for RabbitMq
    /// </summary>
    public string VirtualHost { get; set; }

    /// <summary>
    ///   Дополнительные роутинги из точки обмена к очереди
    /// </summary>
    public string[] Routings { get; set; }
  }
}