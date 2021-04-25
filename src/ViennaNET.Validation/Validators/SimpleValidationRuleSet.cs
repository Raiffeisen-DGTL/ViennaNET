using System.Collections.Generic;
using ViennaNET.Validation.Rules;

namespace ViennaNET.Validation.Validators
{
  internal class SimpleValidationRuleSet<T> : BaseValidationRuleSet<T>
  {
    public SimpleValidationRuleSet(IEnumerable<IRuleBase<T>> rules)
    {
      foreach (var rule in rules)
      {
        SetRule(rule);
      }
    }

    public SimpleValidationRuleSet(IRuleBase<T> rule)
    {
      SetRule(rule);
    }
  }
}
