using System;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.ValidationChains
{
  internal class ValidationChainMemberBuilder<T> : IValidationChainMemberBuilder<T>
  {
    private readonly BaseValidationRuleSet<T> _ruleSet;
    private IValidationChainMember<T> _member;

    public ValidationChainMemberBuilder(BaseValidationRuleSet<T> ruleSet, IValidationChainMember<T> member)
    {
      _ruleSet = ruleSet ?? throw new ArgumentNullException(nameof(ruleSet));
      _member = member ?? throw new ArgumentNullException(nameof(member));
    }

    public IValidationChainMemberBuilder<T> DependsOn(IRuleBase<T> rule)
    {
      _member.DependsOn(_ruleSet.AddMemberToChain(rule));
      return this;
    }

    public IValidationChainMemberBuilder<T> StopOnFailure()
    {
      _member = _ruleSet.SetStopOnFailure(_member);
      return this;
    }
  }
}
