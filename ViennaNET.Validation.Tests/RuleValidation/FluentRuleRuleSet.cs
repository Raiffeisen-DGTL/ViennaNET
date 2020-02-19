using ViennaNET.Validation.Tests.Data;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class FluentRuleRuleSet : BaseValidationRuleSet<IMainEntity>
  {
    public FluentRuleRuleSet(FluentRule rule)
    {
      SetRule(rule);
    }
  }
}