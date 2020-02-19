using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators;
using ViennaNET.Validation.Validators.Exceptions;

namespace ViennaNET.Validation.ValidationChains
{
  internal class ValidationChainMember<T> : IValidationChainMember<T>
  {
    private readonly IList<DependsOnMember> _dependOnRules = new List<DependsOnMember>();
    private readonly IRuleBase<T> _rule;

    public ValidationChainMember(IRuleBase<T> rule)
    {
      _rule = rule ?? throw new ArgumentNullException(nameof(rule));
    }

    public ValidationResult Process(T value, ValidationContext context)
    {
      ValidationResult result = null;

      if (_rule is IRuleAsync<T>)
      {
        throw new ValidationException("Allowed only synchronous rule.");
      }

      if (_rule is IRule<T> rule)
      {
        result = new ValidationResult(rule.Validate(value, context));
      }

      return result;
    }

    public async Task<ValidationResult> ProcessAsync(T value, ValidationContext context)
    {
      ValidationResult result = null;
      if (_rule is IRule<T> rule)
      {
        result = new ValidationResult(rule.Validate(value, context));
      }

      if (_rule is IRuleAsync<T> ruleAsync)
      {
        var resultValidate = await ruleAsync.ValidateAsync(value, context);
        result = new ValidationResult(resultValidate);
      }

      return result;
    }

    public void DependsOn(IValidationChainMember<T> member)
    {
      _dependOnRules.Add(new DependsOnMember(r => r.IsValid, member.Identity));
    }

    public void DependsOn(IValidationChainMember<T> member, Func<ValidationResult, bool> condition)
    {
      _dependOnRules.Add(new DependsOnMember(condition, member.Identity));
    }

    public RuleIdentity Identity => _rule.Identity;

    public IEnumerable<DependsOnMember> DependOnRules => _dependOnRules;
  }
}
