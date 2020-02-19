using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class SecondRuleWithContext : BaseRule<IMainEntity>
  {
    private const string InternalCode = "CPAY12";
    private const string MessageIdentity = "CMES1";

    public SecondRuleWithContext()
      : base(InternalCode)
    {
    }

    public override RuleValidationResult Validate(IMainEntity value, ValidationContext context)
    {
      if ((int) context.Context == 10)
      {
        return new RuleValidationResult(Identity, new ErrorRuleMessage(new MessageIdentity(MessageIdentity), "Error"));
      }
      return null;
    }
  }
}