using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class NotEmptyRuleValidatorAsync : PropertyRuleValidatorAsync<Task<string>>
  {
    protected override async Task<bool> IsValidAsync(Task<string> instance, ValidationContext context)
    {
      var val = await instance;
      return !string.IsNullOrEmpty(val);
    }
  }
}
