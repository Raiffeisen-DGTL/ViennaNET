using System;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.ValidationChains
{
  /// <summary>
  /// Определяет зависимость выполнения правила от условия.
  /// Позволяет задавать цепочки зависимых правил валидации
  /// </summary>
  public class DependsOnMember
  {
    private readonly Func<ValidationResult, bool> _condition;

    /// <summary>
    /// Инициализирует экземпляр ссылками на условие выполнения и идентификатор правила
    /// </summary>
    /// <param name="condition">Ссылка на функцию, по валидационному результату возвращающую булево значение</param>
    /// <param name="ruleIdentity">Идентификатор правила</param>
    public DependsOnMember(Func<ValidationResult, bool> condition, RuleIdentity ruleIdentity)
    {
      _condition = condition ?? throw new ArgumentNullException(nameof(condition));
      RuleIdentity = ruleIdentity ?? throw new ArgumentNullException(nameof(ruleIdentity));
    }

    /// <summary>
    /// Идентификатор правила
    /// </summary>
    public RuleIdentity RuleIdentity { get; }

    /// <summary>
    /// Проверяет, удовлетворяет ли результат валидации сохраненному условию
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool SatisfyCondition(ValidationResult result)
    {
      return _condition.Invoke(result);
    }
  }
}
