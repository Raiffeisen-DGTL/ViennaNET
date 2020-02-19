using System;
using System.Collections.Generic;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class UseIRulePropertyValidator<T> : IRuleValidator<T>
  {
    private readonly IRule<T> _rule;

    public UseIRulePropertyValidator(IRule<T> rule)
    {
      _rule = rule;
    }

    public void AddArguments(IEnumerable<Func<object, object>> func)
    {
    }

    public void SetMessageSource(Func<IRuleMessage> message)
    {
    }

    public void SetState(Func<object, object> state)
    {
    }

    public IList<IRuleMessage> Validate<TEntity>(T instance, TEntity entity, ValidationContext context) => 
      _rule.Validate(instance, context).Messages;
  }
}
