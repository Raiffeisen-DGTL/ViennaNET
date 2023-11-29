using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RabbitMQ.Client;
using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  /// <summary>
  ///   Настройки очереди
  /// </summary>
  public class RabbitMqQueueConfiguration : QueueConfigurationBase, IValidatableObject
  {
    /// <summary>
    ///   Строка подключения (ip или hostname)
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    /// Представляет адреса nodes rabbitmq в формате <!--"ip or hostname":port,"ip or hostname":port-->
    /// </summary>
    public string? Addresses { get; set; }

    /// <summary>
    ///   Порт
    /// </summary>
    public ushort? Port { get; set; }

    /// <summary>
    ///   Пользователь
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    ///   Пароль
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    ///   Тип обмена (Только для 'RabbitMq')
    /// </summary>
    public string ExchangeType { get; set; }

    /// <summary>
    ///   Имя обмена
    /// </summary>
    public string? ExchangeName { get; set; }

    /// <summary>
    ///   Имя очереди
    /// </summary>
    public string? QueueName { get; set; }

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
    public string[]? Routings { get; set; }

    /// <summary>
    /// Автоматически подтверждать сообщения при чтении из очереди
    /// </summary>
    public bool AutoAck { get; set; }

    /// <summary>
    /// Помещать сообщение обратно в очередь при неуспешной обработке
    /// </summary>
    public bool Requeue { get; set; }
    
    /// <summary>
    /// Время ожидания попытки подключения к очереди. По умолчанию 30 секунд.
    /// </summary>
    /// <example>
    /// "0.00:00:30" означает 30 секунд
    /// </example>
    public TimeSpan ConnectionTimeout { get; set; } = ConnectionFactory.DefaultConnectionTimeout;

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (Requeue && AutoAck)
      {
        yield return new ValidationResult("Requeue option is incompatible with AutoAck",
          new[] { nameof(Requeue), nameof(AutoAck) });
      }
    }
  }
}