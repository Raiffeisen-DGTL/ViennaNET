using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.ActiveMQQueue
{
  /// <summary>
  ///   Настройки очереди ActiveMQ
  /// </summary>
  public class ActiveMqQueueConfiguration : QueueConfigurationBase, IValidatableObject
  {
    /// <summary>
    ///   Пользователь (пусто - без авторизации)
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    ///   Пароль
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    ///   Клиентский идентификатор
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    ///   Сервер
    /// </summary>
    public string Server { get; set; }

    /// <summary>
    ///   Порт
    /// </summary>
    public ushort? Port { get; set; }

    /// <summary>
    ///   Признак использования строки подключения
    /// </summary>
    public bool UseQueueString { get; set; }

    /// <summary>
    ///   Строка подключения
    /// </summary>
    public string? QueueString { get; set; }

    /// <summary>
    ///   Имя очереди
    /// </summary>
    public string? QueueName { get; set; }

    /// <summary>
    ///   Признак работы в транзакции
    /// </summary>
    public bool TransactionEnabled { get; set; }

    /// <summary>
    ///   Селектор для вычитывания из очереди отфильтрованных сообщений
    /// </summary>
    public string? Selector { get; set; }

    /// <summary>
    /// Строка подключения к брокеру
    /// </summary>
    [RegularExpression("^(activemq|amqp|failover):.+", 
            ErrorMessage = "URI scheme must be either 'activemq' or 'amqp' or 'failover'")]
    public Uri? ConnectionString { get; set; }

    /// <summary>
    ///  Очередь для ответа
    /// </summary>
    public string? ReplyQueue { get; set; }

    /// <summary>
    /// Время жизни сообщения и время ожидания ответа
    /// </summary>
    public TimeSpan? Lifetime
    {
      get;
      set;
    }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (ConnectionString == null && string.IsNullOrWhiteSpace(Server))
      {
        yield return new ValidationResult("ConnectionString or Server should be set",
          new[] { nameof(ConnectionString), nameof(Server) });
      }

      if (UseQueueString)
      {
        if (string.IsNullOrWhiteSpace(QueueString))
        {
          yield return new ValidationResult("If UseQueueString is true, QueueString should be set",
            new[] { nameof(QueueString) });
        }
      }
      else
      {
        if (string.IsNullOrWhiteSpace(QueueName))
        {
          yield return new ValidationResult("If UseQueueString is false, QueueName should be set",
            new[] { nameof(QueueName) });
        }
      }

      if (TransactionEnabled && ProcessingType != MessageProcessingType.ThreadStrategy)
      {
        yield return new ValidationResult("Only ThreadStrategy is supported with transactions enabled",
          new[] { nameof(ProcessingType) });
      }
    }
  }
}