using ViennaNET.Validation.Rules.FluentRule.RuleValidators;

namespace ViennaNET.Validation.Rules.FluentRule
{
  /// <summary>
  /// Контейнер со ссылкой на последний валидатор в цепи
  /// </summary>
  public abstract class CurrentValidatorHolder<T, TProperty>
  {
    /// <summary>
    /// Cсылка на последний валидатор в цепи
    /// </summary>
    internal abstract IRuleValidatorBase CurrentValidator { get; set; }
  }
}
