using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  internal class UseIRulePropertyValidatorAsync<T> : IRuleValidatorAsync<T>
  {
    private readonly IRuleAsync<T> _rule;

    public UseIRulePropertyValidatorAsync(IRuleAsync<T> rule)
    {
      _rule = rule ?? throw new ArgumentNullException(nameof(rule));
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

    public async Task<IList<IRuleMessage>> ValidateAsync<TEntity>(T instance, TEntity entity, ValidationContext context)
    {
      var valValidate = await _rule.ValidateAsync(instance, context);
      return valValidate.Messages;
    }
  }
}
