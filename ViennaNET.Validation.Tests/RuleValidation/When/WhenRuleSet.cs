using ViennaNET.Validation.Tests.Data;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.Tests.RuleValidation.When
{
  internal class WhenRuleSet : BaseValidationRuleSet<IMainEntity>
  {
    public WhenRuleSet()
    {
      SetRule(new WhenRule());
    }
  }
}