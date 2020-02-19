namespace ViennaNET.Validation.Rules.ValidationResults.RuleMessages
{  
  /// <summary>
  /// Интерфейс валидационного сообщения
  /// </summary>
  public interface IRuleMessage
  {
    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    MessageIdentity Identity { get; }

    /// <summary>
    /// Признак отсутствия ошибки
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Текст ошибки
    /// </summary>
    string Error { get; }

    /// <summary>
    /// Состояние для формирования текста сообщения на основе внешних данных
    /// </summary>
    object State { get; set; }

    /// <summary>
    /// Устанавливает текст сообщения
    /// </summary>
    void SetMessage(string message);

    /// <summary>
    /// Устанавливает аргументы сообщения
    /// </summary>
    void SetArgs(object[] args);

    /// <summary>
    /// Выдает дополнительный текст сообщения
    /// </summary>
    /// <returns>Дополнительный текст сообщения</returns>
    string GetCustomMessageCode();
  }
}