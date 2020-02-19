using System;
using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class LengthValidatorAsync<T> : PropertyRuleValidatorAsync<Task<T>>
  {
    public LengthValidatorAsync(int min, int max)
    {
      Max = max;
      Min = min;
      if (max < min)
      {
        throw new ArgumentOutOfRangeException(nameof(max));
      }
    }

    public int Min { get; }

    public int Max { get; }

    protected override async Task<bool> IsValidAsync(Task<T> instance, ValidationContext context)
    {
      var val = await instance;
      var num = val == null
        ? 0
        : instance.ToString()
          .Length;
      return num >= Min && num <= Max;
    }
  }
}