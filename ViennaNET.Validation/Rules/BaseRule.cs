using System;
using ViennaNET.Validation.Rules.ValidationResults;

namespace ViennaNET.Validation.Rules
{
  /// <summary>
  /// Базовый класс правила, позволяеющего задавать
  /// собственную логику валидации
  /// </summary>
  /// <typeparam name="T">Тип объекта для валидации</typeparam>
  public abstract class BaseRule<T> : IRule<T>
  {
    protected BaseRule()
    {
      Identity = new RuleIdentity(Guid.NewGuid()
                                      .ToString());
    }

    protected BaseRule(string code)
    {
      Identity = new RuleIdentity(code);
    }

    /// <inheritdoc />
    public RuleIdentity Identity { get; }

    /// <inheritdoc />
    public abstract RuleValidationResult Validate(T value, ValidationContext context);
  }
}
