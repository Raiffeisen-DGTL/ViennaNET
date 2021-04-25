using System;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class LengthValidator<T> : PropertyRuleValidator<T>
  {
    public LengthValidator(int min, int max)
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

    protected override bool IsValid(T instance, ValidationContext context)
    {
      var num = instance == null
        ? 0
        : instance.ToString()
                  .Length;
      return num >= Min && num <= Max;
    }
  }
}
