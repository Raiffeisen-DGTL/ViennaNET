using ViennaNET.Validation.Rules.FluentRule;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class FluentRule : BaseFluentRule<IMainEntity>
  {
    public const string InternalCode = "CFL1";
    public const string MessageCode1 = "CFL1M1";
    public const string MessageCode2 = "CFL1M2";
    public const string MessageCode3 = "CFL1M3";
    public const string MessageCode4 = "CFL1M4";
    public const string MessageCode5 = "CFL1M5";

    public FluentRule()
      : base(InternalCode)
    {
      ForProperty(x => x.AccountsInfo).NotNull().WithErrorMessage(MessageCode1, "Отсутствует информация о счете")
        .Must((x, c) => x.Account != null).WithErrorMessage(MessageCode2, "Отсутствует счет")
        .Must((x, c) => x.AccountType != "CurrentInThirdpartyBank").WithErrorMessage(MessageCode5, "Тип счета {0} не должен быть \"В другом банке\"", 
        x => x.AccountsInfo.AccountCba);

      ForProperty(x => x.ActionType).NotEmpty().WithWarningMessage(MessageCode3, "Тип действия не задан")
       .Must((x, c) => x != "Update").WithErrorMessage(MessageCode4, "Типом действия не должно быть обновления");
    }
  }
}