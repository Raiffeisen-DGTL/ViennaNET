using System;
using System.Collections.Generic;
using System.Linq;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  /// <summary>
  /// Позволяет проверять заданное правило на основе публичного интерфейса
  /// проверяемого объекта и вернуть сформированное сообщение
  /// </summary>
  public abstract class PropertyRuleValidator<T> : IRuleValidator<T>
  {
    private readonly List<Func<object, object>> _customFormatArgs = new List<Func<object, object>>();
    private Func<IRuleMessage> _source;
    private Func<object, object> _state;

    /// <summary>
    /// Функции для расчета валидационных аргументов 
    /// </summary>
    public ICollection<Func<object, object>> Arguments => _customFormatArgs;

    /// <inheritdoc />
    public void AddArguments(IEnumerable<Func<object, object>> func) => 
      _customFormatArgs.AddRange(func);

    /// <inheritdoc />
    public void SetMessageSource(Func<IRuleMessage> message) => 
      _source = message ?? throw new ArgumentNullException(nameof(message));

    /// <inheritdoc />
    public void SetState(Func<object, object> state) => _state = state;

    /// <inheritdoc />
    public IList<IRuleMessage> Validate<TEntity>(T instance, TEntity entity, ValidationContext context)
    {
      if (IsValid(instance, context))
      {
        return new List<IRuleMessage>();
      }

      var source = _source.Invoke();
      source.SetArgs(_customFormatArgs.Select(func => func(entity))
                                      .ToArray());
      if (_state != null)
      {
        source.State = _state.Invoke(entity);
      }

      return new[] { source };
    }

    protected abstract bool IsValid(T instance, ValidationContext context);
  }
}
