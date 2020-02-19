namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class HasValueRuleValidator<T> : PropertyRuleValidator<T?> where T : struct
  {
    protected override bool IsValid(T? instance, ValidationContext context)
    {
      return instance.HasValue;
    }
  }
}