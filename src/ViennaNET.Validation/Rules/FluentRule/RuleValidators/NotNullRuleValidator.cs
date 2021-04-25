namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class NotNullRuleValidator<T> : PropertyRuleValidator<T> where T : class
  {
    protected override bool IsValid(T instance, ValidationContext context)
    {
      return instance != null;
    }
  }
}