using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.ValidationChains;

namespace ViennaNET.Validation.Rules.FluentRule
{
  /// <summary>
  /// Базовый класс для асинхронного правила валидации, поддерживающего
  /// текучий интерфейс конфигурирования
  /// </summary>
  /// <typeparam name="T">Тип объекта валидации</typeparam>
  public class BaseFluentRuleAsync<T> : IRuleAsync<T>
  {
    public RuleIdentity Identity { get; }

    private readonly ValidationChain<IRuleValidationMember<T>> _validationChain = new ValidationChain<IRuleValidationMember<T>>();

    protected BaseFluentRuleAsync()
    {
      Identity = new RuleIdentity(Guid.NewGuid()
        .ToString());
    }

    protected BaseFluentRuleAsync(string internalCode)
    {
      Identity = new RuleIdentity(internalCode);
    }

    /// <inheritdoc />
    public async Task<RuleValidationResult> ValidateAsync(T value, ValidationContext context)
    {
      var result = new RuleValidationResult(Identity);
      foreach (var member in _validationChain)
      {
        var validateResults = await member.ValidateAsync(value, context);
        result.AppendRange(validateResults);
      }

      return result;
    }

    /// <summary>
    /// Позволяет начать построение цепи валидаторов правила с текучим интерфейсом
    /// </summary>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="expression">Выражение, возвращающее значение свойства для валидации</param>
    /// <returns>Cтроитель валидаторов правила с текучим интерфейсом</returns>
    protected RuleValidationMemberBuilder<T, TProperty> ForProperty<TProperty>(Expression<Func<T, TProperty>> expression)
    {
      var member = new PropertyRuleValidationMember<T, TProperty>(expression);
      _validationChain.Add(member);
      return new RuleValidationMemberBuilder<T, TProperty>(member);
    }
  }
}