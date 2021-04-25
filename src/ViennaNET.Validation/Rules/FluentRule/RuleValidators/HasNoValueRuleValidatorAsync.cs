using System.Threading.Tasks;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class HasNoValueRuleValidatorAsync<T> : PropertyRuleValidatorAsync<Task<T?>> where T : struct
  {
    protected override async Task<bool> IsValidAsync(Task<T?> instance, ValidationContext context)
    {
      var resultInstance = await instance;
      return !resultInstance.HasValue;
    }
  }
}