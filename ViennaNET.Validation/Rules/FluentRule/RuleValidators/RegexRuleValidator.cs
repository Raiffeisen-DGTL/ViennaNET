using System;
using System.Text.RegularExpressions;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class RegexRuleValidator : PropertyRuleValidator<string>
  {
    private readonly Regex _regex;

    public RegexRuleValidator(string expression)
    {
      if (expression == null)
      {
        throw new ArgumentNullException(nameof(expression));
      }

      _regex = new Regex(expression);
    }

    public RegexRuleValidator(Regex expression)
    {
      _regex = expression ?? throw new ArgumentNullException(nameof(expression));
    }


    protected override bool IsValid(string instance, ValidationContext context)
    {
      return string.IsNullOrEmpty(instance) || _regex.IsMatch(instance);
    }
  }
}
