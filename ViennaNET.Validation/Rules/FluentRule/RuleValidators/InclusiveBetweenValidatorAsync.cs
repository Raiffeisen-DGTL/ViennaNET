using System;
using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class InclusiveBetweenValidatorAsync<T> : PropertyRuleValidatorAsync<Task<T>> where T : IComparable
  {
    private readonly IComparable _from;

    private readonly IComparable _to;

    public InclusiveBetweenValidatorAsync(IComparable from, IComparable to)
    {
      _from = from ?? throw new ArgumentNullException(nameof(from));
      _to = to ?? throw new ArgumentNullException(nameof(to));

      if (to.CompareTo(_from) == -1)
      {
        throw new ArgumentOutOfRangeException(nameof(to), "To should be larger than from.");
      }
    }

    protected override async Task<bool> IsValidAsync(Task<T> instance, ValidationContext context)
    {
      T result = await instance;
      return result == null || result.CompareTo(_from) >= 0 && result.CompareTo(_to) <= 0;
    }
  }
}