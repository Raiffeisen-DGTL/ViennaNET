using ViennaNET.Validation.Tests.Data;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class SalaryLoadingRuleSet : BaseValidationRuleSet<ICollectionEntity>
  {
    public SalaryLoadingRuleSet()
    {
      SetRule(new CheckAmountRule());
    }
  }
}