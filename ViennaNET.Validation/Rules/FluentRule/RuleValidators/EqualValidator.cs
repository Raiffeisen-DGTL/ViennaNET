using System.Collections.Generic;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class EqualValidator<T> : PropertyRuleValidator<T>
  {
    private readonly IEqualityComparer<T> _comparer;
    private readonly T _valueToCompare;

    public EqualValidator(T valueToCompare, IEqualityComparer<T> comparer)
    {
      _valueToCompare = valueToCompare;
      _comparer = comparer;
    }

    protected bool Compare(T comparisonValue, T propertyValue) =>
      _comparer?.Equals(comparisonValue, propertyValue) ?? Equals(comparisonValue, propertyValue);

    protected override bool IsValid(T instance, ValidationContext context)
    {
      return Compare(instance, _valueToCompare);
    }
  }
}