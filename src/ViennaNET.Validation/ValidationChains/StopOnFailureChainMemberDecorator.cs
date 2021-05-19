using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators;
using ViennaNET.Validation.Validators.Exceptions;

namespace ViennaNET.Validation.ValidationChains
{
  internal class StopOnFailureChainMemberDecorator<T> : IValidationChainMember<T>
  {
    private readonly IValidationChainMember<T> _member;

    public StopOnFailureChainMemberDecorator(IValidationChainMember<T> member)
    {
      _member = member ?? throw new ArgumentNullException(nameof(member));
    }

    public ValidationResult Process(T value, ValidationContext context)
    {
      var result = _member.Process(value, context);
      if (!result.IsValid)
      {
        throw new ValidationStoppedException(result);
      }
      return result;
    }

    public async Task<ValidationResult> ProcessAsync(T value, ValidationContext context)
    {
      var result = await _member.ProcessAsync(value, context);
      if (!result.IsValid)
      {
        throw new ValidationStoppedException(result);
      }

      return result;
    }

    public void DependsOn(IValidationChainMember<T> member)
    {
      _member.DependsOn(member);
    }

    public void DependsOn(IValidationChainMember<T> member, Func<ValidationResult, bool> condition)
    {
      _member.DependsOn(member, condition);
    }

    public RuleIdentity Identity => _member.Identity;

    public IEnumerable<DependsOnMember> DependOnRules => _member.DependOnRules;
  }
}
