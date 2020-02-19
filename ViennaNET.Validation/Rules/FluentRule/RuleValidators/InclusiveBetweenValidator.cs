using System;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class InclusiveBetweenValidator<T> : PropertyRuleValidator<T> where T : IComparable
  {
    private readonly IComparable _from;

    private readonly IComparable _to;

    public InclusiveBetweenValidator(IComparable from, IComparable to)
    {
      _from = from ?? throw new ArgumentNullException(nameof(from));
      _to = to ?? throw new ArgumentNullException(nameof(to));

      if (to.CompareTo(_from) == -1)
      {
        throw new ArgumentOutOfRangeException(nameof(to), "To should be larger than from.");
      }
    }

    protected override bool IsValid(T instance, ValidationContext context)
    {
      return instance == null || instance.CompareTo(_from) >= 0 && instance.CompareTo(_to) <= 0;
    }
  }
}
