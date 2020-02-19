using System;
using System.Linq.Expressions;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.ValidationChains;

namespace ViennaNET.Validation.Rules.FluentRule
{
  /// <summary>
  /// Базовый класс для правила валидации, поддерживающего
  /// текучий интерфейс конфигурирования
  /// </summary>
  /// <typeparam name="T">Тип объекта валидации</typeparam>
  public abstract class BaseFluentRule<T> : IRule<T>
  {
    private readonly ValidationChain<IRuleValidationMember<T>> _validationChain = new ValidationChain<IRuleValidationMember<T>>();

    protected BaseFluentRule()
    {
      Identity = new RuleIdentity(Guid.NewGuid()
                                      .ToString());
    }

    protected BaseFluentRule(string internalCode)
    {
      Identity = new RuleIdentity(internalCode);
    }

    /// <inheritdoc />
    public RuleIdentity Identity { get; }

    /// <inheritdoc />
    public RuleValidationResult Validate(T value, ValidationContext context)
    {
      var result = new RuleValidationResult(Identity);
      foreach (var member in _validationChain)
      {
        result.AppendRange(member.Validate(value, context));
      }
      return result;
    }

    protected RuleValidationMemberBuilder<T, TProperty> ForProperty<TProperty>(Expression<Func<T, TProperty>> expression)
    {
      var member = new PropertyRuleValidationMember<T, TProperty>(expression);
      _validationChain.Add(member);
      return new RuleValidationMemberBuilder<T, TProperty>(member);
    }
  }
}
