using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Mediator.Tests.Units.Infrastructure
{
  class TestValidator : BaseRule<int>, IMessageValidation
  {
    public ValidationResult Validate(object message)
    {
      return null;
    }

    public override RuleValidationResult Validate(int value, ValidationContext context)
    {
      return null;
    }
  }
}
