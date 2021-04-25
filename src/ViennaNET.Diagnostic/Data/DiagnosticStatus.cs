namespace ViennaNET.Diagnostic.Data
{
  /// <summary>
  /// Статус диагностики функции
  /// </summary>
  public enum DiagnosticStatus
  {
    /// <summary>
    /// Успешно. Может выступать как статусом конкретной
    /// функции, так и обобщающим статусом
    /// </summary>
    Ok,

    /// <summary>
    /// Ошибка. Обобщающий статус
    /// </summary>
    Fail,

    /// <summary>
    /// Ошибка конфигурации
    /// </summary>
    ConfigError,

    /// <summary>
    /// Ошибка подключения к БД
    /// </summary>
    DbConnectionError,

    /// <summary>
    /// Ошибка запроса к стороннему сервису
    /// </summary>
    PingError,

    /// <summary>
    /// Ошибка запроса к очереди
    /// </summary>
    QueueError,

    /// <summary>
    /// Неизвестная ошибка
    /// </summary>
    UnknownError,
  }
}
