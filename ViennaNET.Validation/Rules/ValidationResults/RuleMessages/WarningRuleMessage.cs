namespace ViennaNET.Validation.Rules.ValidationResults.RuleMessages
{
  /// <summary>
  /// Предупреждение
  /// </summary>
  public class WarningRuleMessage : BaseRuleMessage
  {
    /// <summary>
    /// Инициализирует экземпляр ссылками на параметры
    /// </summary>
    /// <param name="identity">Идентификатор сообщения</param>
    /// <param name="error">Текст ошибки</param>
    /// <param name="args">Аргументы для форматирования текста</param>
    public WarningRuleMessage(MessageIdentity identity, string error, params object[] args) : base(null, identity, null, error, args)
    {
    }

    /// <summary>
    /// Инициализирует экземпляр ссылками на параметры
    /// </summary>
    /// <param name="state">Состояние для формирования текста сообщения на основе внешних данных</param>
    /// <param name="identity">Идентификатор сообщения</param>
    /// <param name="error">Текст ошибки</param>
    /// <param name="args">Аргументы для форматирования текста</param>
    public WarningRuleMessage(object state, MessageIdentity identity, string error, params object[] args) :
      base(state, identity, null, error, args)
    {
    }

    /// <summary>
    /// Инициализирует экземпляр ссылками на параметры
    /// </summary>
    /// <param name="state">Состояние для формирования текста сообщения на основе внешних данных</param>
    /// <param name="customCode">Дополнительный код сообщения</param>
    /// <param name="identity">Идентификатор сообщения</param>
    /// <param name="error">Текст ошибки</param>
    /// <param name="args">Аргументы для форматирования текста</param>
    public WarningRuleMessage(object state, string customCode, MessageIdentity identity, string error, params object[] args) :
      base(state, identity, customCode, error, args)
    {
    }

    /// <summary>
    /// Инициализирует экземпляр ссылками на параметры
    /// </summary>
    /// <param name="customCode">Дополнительный код сообщения</param>
    /// <param name="identity">Идентификатор сообщения</param>
    /// <param name="error">Текст ошибки</param>
    /// <param name="args">Аргументы для форматирования текста</param>
    public WarningRuleMessage(string customCode, MessageIdentity identity, string error, params object[] args) :
      base(null, identity, customCode, error, args)
    {
    }

    /// <inheritdoc />
    public override bool IsValid => true;
  }
}
