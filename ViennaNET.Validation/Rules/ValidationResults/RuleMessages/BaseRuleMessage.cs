using System;

namespace ViennaNET.Validation.Rules.ValidationResults.RuleMessages
{
  /// <summary>
  /// Базовый класс валидационного сообщения. Предоставляет
  /// реализацию основных свойств и методов
  /// </summary>
  public abstract class BaseRuleMessage : IRuleMessage
  {
    private object[] _args;
    private string _message;

    protected BaseRuleMessage(object state, MessageIdentity identity, string customCode, string message, params object[] args)
    {
      Identity = identity ?? throw new ArgumentNullException(nameof(identity));
      CustomCode = customCode;
      State = state;
      _message = message;
      _args = args;
    }

    /// <summary>
    /// Дополнительный код сообщения
    /// </summary>
    public string CustomCode { get; }

    /// <inheritdoc />
    public MessageIdentity Identity { get; }

    /// <inheritdoc />
    public abstract bool IsValid { get; }

    /// <inheritdoc />
    public object State { get; set; }

    /// <inheritdoc />
    public string Error => string.Format(_message, _args);

    /// <inheritdoc />
    public void SetMessage(string message)
    {
      _message = message;
    }

    /// <inheritdoc />
    public void SetArgs(object[] args)
    {
      _args = args;
    }

    /// <inheritdoc />
    public string GetCustomMessageCode()
    {
      return CustomCode;
    }
  }
}
