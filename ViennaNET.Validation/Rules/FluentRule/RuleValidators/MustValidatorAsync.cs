using System;
using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class MustValidatorAsync<T> : PropertyRuleValidatorAsync<Task<T>>
  {
    private readonly Func<Task<T>, object, Task<bool>> _func;

    public MustValidatorAsync(Func<Task<T>, object, Task<bool>> func) =>
      _func = func ?? throw new ArgumentNullException(nameof(func));

    protected override Task<bool> IsValidAsync(Task<T> instance, ValidationContext context)
    {
      return _func(instance, context?.Context);
    }
  }
}