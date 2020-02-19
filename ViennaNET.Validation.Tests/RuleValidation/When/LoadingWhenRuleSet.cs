using ViennaNET.Validation.Tests.Data;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.Tests.RuleValidation.When
{
  internal class LoadingWhenRuleSet : BaseValidationRuleSet<IMainEntity>
  {
    public LoadingWhenRuleSet(IDbAccessor accessor)
    {
      When(new AccountTypeRule(accessor),
        () => SetRule(new ContractAccountRule(accessor)));
    }
  }
}