using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class AccountTypeRule : BaseRule<IMainEntity>
  {
    private readonly IDbAccessor _dbAccessor;
    private const string InternalCode = "CPAY1";

    public AccountTypeRule(IDbAccessor accessor)
      : base(InternalCode)
    {
      _dbAccessor = accessor;
    }

    public override RuleValidationResult Validate(IMainEntity entity, ValidationContext context)
    {
      if (entity.DocType != "Excel")
      {
        return null;
      }
      var res = entity.AccountsInfo.AccountType == "Transit" ||
                   entity.AccountsInfo.AccountType == "CurrentInTheBank";
      if (res)
      {
        return null;
      }
      var type = _dbAccessor.GetContractAccountType(entity.AccountsInfo.AccountType);
      return new RuleValidationResult(Identity, new ErrorRuleMessage(new MessageIdentity("CPAY1M1"),
        string.Format(
          "Тип счета {0} - {1}. Использование данного типа счета в ведомости недопустимо.",
          entity.AccountsInfo.AccountCba,
          type)));
    }
  }
}