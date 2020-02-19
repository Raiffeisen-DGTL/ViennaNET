using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class HasValueRuleValidatorAsync<T> : PropertyRuleValidatorAsync<Task<T?>> where T : struct
  {
    protected override async Task<bool> IsValidAsync(Task<T?> instance, ValidationContext context)
    {
      var valInstance = await instance;
      return valInstance.HasValue;
    }
  }
}