using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  /// <summary>
  /// Место хранения сертификата
  /// </summary>
  public enum ClientCertificateStore
  {
    /// <summary>
    /// User
    /// </summary>
    User = 0,

    /// <summary>
    /// Machine
    /// </summary>
    Machine,
  }

  /// <summary>
  ///   Настройки очереди
  /// </summary>
  public class MqSeriesQueueConfiguration : QueueConfigurationBase, IValidatableObject
  {
    /// <summary>
    ///   Клиентский идентификатор
    /// </summary>
    [Required]
    public string ClientId { get; set; }

    /// <summary>
    ///   Сервер
    /// </summary>
    [Required]
    public string Server { get; set; }

    /// <summary>
    ///   Порт
    /// </summary>
    [Required]
    public ushort Port { get; set; }

    /// <summary>
    ///   Имя менеджера очередей
    /// </summary>
    [Required]
    public string QueueManager { get; set; }

    /// <summary>
    ///   Имя канала
    /// </summary>
    [Required]
    public string Channel { get; set; }

    /// <summary>
    ///   Пользователь (пусто - без авторизации)
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    ///   Пароль
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    ///   Селектор для вычитывания из очереди отфильтрованных сообщений
    /// </summary>
    public string? Selector { get; set; }

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
    ///   Признак использования строки подключения
    /// </summary>
    public bool UseQueueString { get; set; }

    /// <summary>
    /// Использовать защищённое соединение с брокером
    /// </summary>
    public bool TlsEnabled { get; set; }

    /// <summary>
    /// Имя хранилища сертификатов для защищённого подключения к брокеру
    /// </summary>
    public ClientCertificateStore TlsClientCertStore { get; set; }

    /// <summary>
    /// Friendly Name клиентского сертификата на Windows
    /// </summary>
    public string? TlsClientCertLabel { get; set; }

    /// <summary>
    /// Subject (DN) сертификата сервера для проверки при подключении
    /// </summary>
    public string? TlsServerCertSubject { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
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

      if (!TlsEnabled)
      {
        if (!string.IsNullOrEmpty(TlsClientCertLabel))
        {
          yield return new ValidationResult("Need to enable TLS first: set TlsEnabled to true",
            new[] { nameof(TlsClientCertLabel) });
        }

        if (!string.IsNullOrEmpty(TlsServerCertSubject))
        {
          yield return new ValidationResult("Need to enable TLS first: set TlsEnabled to true",
            new[] { nameof(TlsServerCertSubject) });
        }
      }
    }
  }
}