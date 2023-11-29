using ViennaNET.Validation.Tests.Data;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class LoadingDependsOnRuleSet : BaseValidationRuleSet<IMainEntity>
  {
    public LoadingDependsOnRuleSet(IDbAccessor accessor)
    {
      SetRule(new ContractAccountRule(accessor)).DependsOn(new AccountTypeRule(accessor));

      SetCollectionContext(x => x.Salaries, new SalaryLoadingRuleSet());
    }
  }
}