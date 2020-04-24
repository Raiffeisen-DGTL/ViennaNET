using Confluent.Kafka;
using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.KafkaQueue
{
  /// <summary>
  /// Настройки очереди
  /// </summary>
  public class KafkaQueueConfiguration : QueueConfigurationBase
  {
    /// <summary>
    /// Определяет, будет ли адаптер потребителем (True) либо отправителем (False) 
    /// </summary>
    public bool IsConsumer { get; set; }

    /// <summary>
    /// Имя сервиса
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// Путь до файла keytab
    /// </summary>
    public string KeyTab { get; set; }

    /// <summary>
    /// Протокол безопасности
    /// </summary>
    public SecurityProtocol? Protocol { get; set; }

    /// <summary>
    /// Механизм безопасности
    /// </summary>
    public SaslMechanism? Mechanism { get; set; }

    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public string GroupId { get; set; }

    /// <summary>
    /// Строка с настройкой режима логирования
    /// </summary>
    public string Debug { get; set; }

    /// <summary>
    /// Действие, которое необходимо предпринять, в случае если оффсета нет в харнилище либо он выходит за допустимые границы
    /// </summary>
    public AutoOffsetReset? AutoOffsetReset { get; set; }
  }
}
