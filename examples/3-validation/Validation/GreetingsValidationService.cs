using ValidationService.Validation.Contexts;
using ValidationService.Validation.Rulesets;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators;

namespace ValidationService.Validation
{
  public class GreetingsValidationService : IGreetingsValidationService
  {
    private readonly GreetingsRuleset _ruleset;
    private readonly IValidator _validator;

    public GreetingsValidationService(IValidator validator, GreetingsRuleset ruleset)
    {
      _validator = validator;
      _ruleset = ruleset;
    }

    public ValidationResult ValidateGreeting(string greeting)
    {
      var context = new ValidationContext(new GreetingValidationContext());
      return _validator.Validate(_ruleset, greeting, context);
    }
  }
}