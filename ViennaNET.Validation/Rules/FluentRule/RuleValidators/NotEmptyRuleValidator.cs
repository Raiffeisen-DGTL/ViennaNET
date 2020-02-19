namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class NotEmptyRuleValidator : PropertyRuleValidator<string>
  {
    protected override bool IsValid(string instance, ValidationContext context)
    {
      return !string.IsNullOrEmpty(instance);
    }
  }
}
