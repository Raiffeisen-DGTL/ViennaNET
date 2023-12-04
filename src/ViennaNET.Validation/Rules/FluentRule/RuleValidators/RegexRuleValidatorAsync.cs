using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class RegexRuleValidatorAsync : PropertyRuleValidatorAsync<Task<string>>
  {
    private readonly Regex _regex;

    public RegexRuleValidatorAsync(string expression)
    {
      if (expression == null)
      {
        throw new ArgumentNullException(nameof(expression));
      }

      _regex = new Regex(expression);
    }

    public RegexRuleValidatorAsync(Regex expression)
    {
      _regex = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    protected override async Task<bool> IsValidAsync(Task<string> instance, ValidationContext context)
    {
      string val = await instance;
      return string.IsNullOrEmpty(val) || _regex.IsMatch(val);
    }
  }
}