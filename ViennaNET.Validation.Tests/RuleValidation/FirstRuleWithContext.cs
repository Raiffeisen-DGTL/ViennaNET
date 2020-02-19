using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class FirstRuleWithContext : BaseRule<IMainEntity>
  {
    private const string InternalCode = "CPAY11";

    public FirstRuleWithContext()
      : base(InternalCode)
    {
    }

    public override RuleValidationResult Validate(IMainEntity value, ValidationContext context)
    {
      context.Context = 10;
      return null;
    }
  }
}