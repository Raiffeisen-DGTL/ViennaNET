using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules.FluentRule.RuleValidators;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.FluentRule
{
  internal class PropertyRuleValidationMember<T, TProperty> : IRuleValidationMember<T>
  {
    private readonly object _criteriaLocker = new object();
    private readonly Func<T, TProperty> _propertyGetter;
    private readonly List<IRuleValidatorBase> _validators;

    private Func<object, object, bool> _criteria;
    private Func<object, object, Task<bool>> _criteriaAsync;

    public PropertyRuleValidationMember(Expression<Func<T, TProperty>> expression)
    {
      if (expression == null)
      {
        throw new ArgumentNullException(nameof(expression));
      }

      _validators = new List<IRuleValidatorBase>();
      _propertyGetter = expression.Compile();
    }

    public IEnumerable<IRuleValidatorBase> Validators => _validators;

    public IEnumerable<IRuleMessage> Validate(T instance, ValidationContext context)
    {
      if (Validators.Any(x => x is IRuleValidatorAsync<TProperty>))
      {
        throw new ArgumentException("Validators have async item! Use ValidateAsync method or change your rule set");
      }

      var result = new List<IRuleMessage>();

      if (_criteria != null)
      {
        lock (_criteriaLocker)
        {
          if (!_criteria.Invoke(instance, context?.Context))
          {
            return result;
          }
        }
      }

      foreach (var ruleValidator in _validators)
      {
        if (ruleValidator is IRuleValidator<TProperty> validator)
        {
          result.AddRange(validator.Validate(GetProperty(instance), instance, context));
        }
      }

      return result;
    }

    private TProperty GetProperty(T instance)
    {
      lock (_propertyGetter)
      {
        var val = _propertyGetter.Invoke(instance);
        return val;
      }
    }

    public async Task<IEnumerable<IRuleMessage>> ValidateAsync(T instance, ValidationContext context)
    {
      var result = new List<IRuleMessage>();

      if (_criteriaAsync != null)
      {
        if (!await _criteriaAsync.Invoke(instance, context?.Context))
        {
          return result;
        }
      }

      foreach (var ruleValidator in _validators)
      {
        if (ruleValidator is IRuleValidator<TProperty> validator)
        {
          result.AddRange(validator.Validate(GetProperty(instance), instance, context));
        }
        else if (ruleValidator is IRuleValidatorAsync<TProperty> validatorAsync)
        {
          var results = await validatorAsync.ValidateAsync(GetProperty(instance), instance, context);
          result.AddRange(results);
        }
      }

      return result;
    }

    public void ApplyCondition(Func<object, object, bool> condition)
    {
      lock (_criteriaLocker)
      {
        _criteria = condition;
        _criteriaAsync = (x, c) => Task.FromResult(condition(x, c));
      }
    }

    public void ApplyCondition(Func<object, object, Task<bool>> condition)
    {
      lock (_criteriaLocker)
      {
        _criteriaAsync = condition;
      }
    }

    public void AddValidator(IRuleValidatorBase validator)
    {
      _validators.Add(validator);
    }
  }
}
