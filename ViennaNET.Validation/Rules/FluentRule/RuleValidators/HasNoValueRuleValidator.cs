namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class HasNoValueRuleValidator<T> : PropertyRuleValidator<T?> where T : struct
  {
    protected override bool IsValid(T? instance, ValidationContext context)
    {
      return !instance.HasValue;
    }
  }
}
