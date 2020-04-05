using ValidationService.Validation.Rules;
using ViennaNET.Validation.Validators;

namespace ValidationService.Validation.Rulesets
{
  public class GreetingsRuleset : BaseValidationRuleSet<string>
  {
    public GreetingsRuleset(LengthRule lengthRule, StartsWithRule startsWithRule)
    {
      SetRule(lengthRule)
        .StopOnFailure();

      SetRule(startsWithRule)
        .StopOnFailure();
    }
  }
}
