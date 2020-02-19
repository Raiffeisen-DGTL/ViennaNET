using System;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class MustValidator<T> : PropertyRuleValidator<T>
  {
    private readonly Func<T, object, bool> _func;

    public MustValidator(Func<T, object, bool> func) => 
      _func = func ?? throw new ArgumentNullException(nameof(func));

    protected override bool IsValid(T instance, ValidationContext context) => 
      _func.Invoke(instance, context?.Context);
  }
}
