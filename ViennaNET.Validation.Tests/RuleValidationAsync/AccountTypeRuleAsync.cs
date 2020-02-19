using System.Threading.Tasks;
using ViennaNET.Validation.Rules.FluentRule;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidationAsync
{
  public class AccountTypeRuleAsync : BaseFluentRuleAsync<IAccInfo>
  {
    public AccountTypeRuleAsync()
    {
      ForProperty(a => Task.FromResult(a.Account))
        .Must((x, c) => Task.FromResult(false))
        .WithErrorMessage("Account is null");
    }
  }
}