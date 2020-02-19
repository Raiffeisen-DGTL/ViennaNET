using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.MessageFormatting
{
  /// <summary>
  /// Расширения для правила форматирования
  /// </summary>
  public static class MessageFormatRuleExtensions
  {
    /// <summary>
    /// Устанавливает идентификатор сообщения правила для форматирования
    /// </summary>
    /// <param name="rule">Ссылка на правило форматирования</param>
    /// <param name="identity">Ссылка на идентификатор сообщения</param>
    /// <returns>Ссылка на правило форматирования</returns>
    public static MessageFormatRule ForMessage(this MessageFormatRule rule, string identity)
    {
      rule.SetMessageIdentity(new MessageIdentity(identity));
      return rule;
    }

    /// <summary>
    /// Устанавливает сообщение для замены существующего
    /// </summary>
    /// <param name="rule">Ссылка на правило форматирования</param>
    /// <param name="message">Сообщение для замены</param>
    /// <returns>Ссылка на правило форматирования</returns>
    public static void OverrideMessage(this MessageFormatRule rule, string message)
    {
      rule.SetOverridingMessage(message); 
    }
  }
}
