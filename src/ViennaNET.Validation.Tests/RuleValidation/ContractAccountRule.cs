using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class ContractAccountRule : BaseRule<IMainEntity>
  {
    private readonly IDbAccessor _dbAccessor;
    private const string InternalCode = "CPAY2";

    public ContractAccountRule(IDbAccessor accessor)
      : base(InternalCode)
    {
      _dbAccessor = accessor;
    }

    public override RuleValidationResult Validate(IMainEntity entity, ValidationContext context)
    {
      if (_dbAccessor.GetContractAccount(entity.ContractId, entity.AccountsInfo.AccountCba) == null)
      {
        return new RuleValidationResult(Identity, new ErrorRuleMessage(new MessageIdentity("CPAY2M1"),
          string.Format("Не найден счёт {0} для договора {1}",
            entity.AccountsInfo.AccountCba,
            entity.ContractId)));
      }
      return null;
    }
  }
}