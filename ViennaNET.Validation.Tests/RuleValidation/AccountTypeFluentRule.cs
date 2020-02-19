using ViennaNET.Validation.Rules.FluentRule;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  class AccountTypeFluentRule : BaseFluentRule<IAccInfo>
  {
    public AccountTypeFluentRule()
    {
      ForProperty(a => a.Account)
        .NotNull()
        .WithErrorMessage("Account is null");
      ForProperty(a => a.AccountCba)
        .NotNull()
        .WithErrorMessage("AccountCba is null");
      ForProperty(a => a.AccountType)
        .NotNull()
        .WithErrorMessage("AccountType is null");
    }
  }
}
