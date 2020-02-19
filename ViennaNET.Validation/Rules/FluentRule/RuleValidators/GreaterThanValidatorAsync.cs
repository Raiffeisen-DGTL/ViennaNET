using System;
using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class GreaterThanValidatorAsync<T> : PropertyRuleValidatorAsync<T> where T : IComparable
  {
    private readonly Task<T> _valueToCompare;

    public GreaterThanValidatorAsync(Task<T> valueToCompare)
    {
      _valueToCompare = valueToCompare;
    }

    protected int Compare(T comparisonValue, T propertyValue) => comparisonValue.CompareTo(_valueToCompare);

    protected override async Task<bool> IsValidAsync(T instance, ValidationContext context)
    {
      return Compare(instance, await _valueToCompare) > 0;
    }
  }
}
