using System.Linq;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;
using ViennaNET.Validation.Validators;
using ViennaNET.Validation.Validators.Exceptions;

namespace ViennaNET.Validation.MessageFormatting
{
  /// <summary>
  /// Правило для форматирования сообщений валидации
  /// </summary>
  public sealed class MessageFormatRule
  {
    private readonly RuleIdentity _ruleIdentity;

    private MessageIdentity _messageIdentity;
    private string _overridingMessage;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="RuleIdentity"/>
    /// </summary>
    /// <param name="ruleIdentity">Ссылка на идентификатор правила</param>
    public MessageFormatRule(RuleIdentity ruleIdentity)
    {
      _ruleIdentity = ruleIdentity;
    }

    /// <summary>
    /// Устанавливает идентификатор сообщения правила для форматирования
    /// </summary>
    /// <param name="identity">Ссылка на идентификатор сообщения</param>
    public void SetMessageIdentity(MessageIdentity identity)
    {
      _messageIdentity = identity;
    }

    /// <summary>
    /// Устанавливает сообщение для замены существующего
    /// </summary>
    /// <param name="message">Сообщение для замены</param>
    public void SetOverridingMessage(string message)
    {
      _overridingMessage = message;
    }

    /// <summary>
    /// Выполняет форматирование сообщения правила по <see cref="RuleIdentity"/> с
    /// заданным <see cref="MessageIdentity"/>, заменяя его текст
    /// </summary>
    /// <param name="result">Результат валидации для форматирования</param>
    /// <returns>Результат валидации с замененными текстами сообщений</returns>
    /// <exception cref="ValidationException">Исключение при невозможности определить сообщение для замены текста</exception>
    public ValidationResult Format(ValidationResult result)
    {
      if (_messageIdentity == null)
      {
        var ruleValidationResult = result.Results.FirstOrDefault(x => x.RuleIdentity == _ruleIdentity);
        if (ruleValidationResult == null)
        {
          return result;
        }
        if (ruleValidationResult.Messages.Count > 1)
        {
          throw new
            ValidationException($"Для правила с кодом {_ruleIdentity.Code} нельзя заменить текст сообщения, поскольку правило вернуло несколько сообщений");
        }
        var message = ruleValidationResult.Messages.FirstOrDefault();
        if (message == null)
        {
          return result;
        }
        message.SetMessage(_overridingMessage);
      }
      else
      {
        var ruleValidationResult = result.Results.FirstOrDefault(x => x.RuleIdentity == _ruleIdentity);
        if (ruleValidationResult == null)
        {
          return result;
        }
        var message = ruleValidationResult.Messages.FirstOrDefault(x => x.Identity == _messageIdentity);
        if (message == null)
        {
          return result;
        }
        message.SetMessage(_overridingMessage);
      }
      return result;
    }
  }
}
