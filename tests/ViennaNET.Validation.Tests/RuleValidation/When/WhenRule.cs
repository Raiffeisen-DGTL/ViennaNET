using ViennaNET.Validation.Rules.FluentRule;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidation.When
{
  internal class WhenRule : BaseFluentRule<IMainEntity>
  {
    private const string internalCode = "CFL2";
    private const string messageCode1 = "CFL2M1";

    public WhenRule()
      : base(internalCode)
    {
      ForProperty(x => x.AccountsInfo)
        .Must((x, c) => x.AccountCba == "1234567890")
        .WithWarningMessage(messageCode1, "Номер счёта некорректен")
        .When((x, c) => x.AccountsInfo.AccountType == "CurrentInThirdpartyBank");
    }
  }
}