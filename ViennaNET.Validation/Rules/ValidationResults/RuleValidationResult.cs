using System;
using System.Collections.Generic;
using System.Linq;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.ValidationResults
{
  /// <summary>
  /// Результат валидации правила
  /// </summary>
  public sealed class RuleValidationResult
  {
    private readonly List<IRuleMessage> _messages;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на идентификатор правила
    /// </summary>
    /// <param name="identity">Идентификатор сообщения</param>
    public RuleValidationResult(RuleIdentity identity)
    {
      RuleIdentity = identity ?? throw new ArgumentNullException(nameof(identity));
      _messages = new List<IRuleMessage>();
    }

    /// <summary>
    /// Инициализирует экземпляр ссылками на идентификатор правила и сообщение
    /// </summary>
    /// <param name="identity">Идентификатор сообщения</param>
    /// <param name="message">Валидационное сообщение</param>
    public RuleValidationResult(RuleIdentity identity, IRuleMessage message) : this(identity)
    {
      _messages.Add(message);
    }

    /// <summary>
    /// Признак отсутствия ошибки
    /// </summary>
    public bool IsValid => !Messages.Any(x => x is ErrorRuleMessage);

    /// <summary>
    /// Возвращает коллекци сообщений
    /// </summary>
    public IList<IRuleMessage> Messages => _messages;

    /// <summary>
    /// Идентификатор правила
    /// </summary>
    public RuleIdentity RuleIdentity { get; }

    /// <summary>
    /// Позволяет добавить новое сообщение в коллеккию
    /// </summary>
    /// <param name="message">Сообщение</param>
    public void Append(IRuleMessage message)
    {
      if (message == null)
      {
        return;
      }
      _messages.Add(message);
    }

    /// <summary>
    /// Позволяет добавить коллеккцию новых сообщений
    /// </summary>
    /// <param name="message">Коллекция сообщений</param>
    public void AppendRange(IEnumerable<IRuleMessage> messages) => 
      _messages.AddRange(messages);
  }
}
