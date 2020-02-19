using System.Collections.Generic;
using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class EqualValidatorAsync<T> : PropertyRuleValidatorAsync<T>
  {
    private readonly IEqualityComparer<T> _comparer;
    private readonly Task<T> _valueToCompare;

    public EqualValidatorAsync(Task<T> valueToCompare, IEqualityComparer<T> comparer)
    {
      _valueToCompare = valueToCompare;
      _comparer = comparer;
    }

    protected override async Task<bool> IsValidAsync(T instance, ValidationContext context)
    {
      T compareValue = await _valueToCompare;
      return Compare(instance, compareValue);
    }

    protected bool Compare(T comparisonValue, T propertyValue) =>
      _comparer?.Equals(comparisonValue, propertyValue) ?? Equals(comparisonValue, propertyValue);
  }
}