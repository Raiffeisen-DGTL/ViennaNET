using System;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules.ValidationResults;

namespace ViennaNET.Validation.Rules
{
  /// <summary>
  /// Базовый класс правила, позволяеющего задавать
  /// собственную логику валидации
  /// </summary>
  /// <typeparam name="T">Тип объекта для валидации</typeparam>
  public abstract class BaseRuleAsync<T> : IRuleAsync<T>
  {
    protected BaseRuleAsync()
    {
      Identity = new RuleIdentity(Guid.NewGuid()
        .ToString());
    }

    protected BaseRuleAsync(string code)
    {
      Identity = new RuleIdentity(code);
    }

    /// <inheritdoc />
    public RuleIdentity Identity { get; }

    /// <inheritdoc />
    public abstract Task<RuleValidationResult> ValidateAsync(T value, ValidationContext context);
  }
}