using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class NullRuleValidatorAsync<T> : PropertyRuleValidatorAsync<Task<T>> where T : class
  {
    protected override async Task<bool> IsValidAsync(Task<T> instance, ValidationContext context)
    {
      T val = await instance;
      return val == null;
    }
  }
}