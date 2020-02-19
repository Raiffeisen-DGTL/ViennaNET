using ViennaNET.Validation.Rules.FluentRule;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidationAsync
{
  class UseIRuleFluentRuleAsync : BaseFluentRuleAsync<IMainEntity>
  {
    public UseIRuleFluentRuleAsync()
    {
      ForProperty(e => e.AccountsInfo)
        .UseRuleAsync(new AccountTypeRuleAsync());
    }
  }
}