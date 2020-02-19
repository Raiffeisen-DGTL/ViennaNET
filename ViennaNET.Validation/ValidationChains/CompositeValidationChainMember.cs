using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.ValidationChains
{
  internal class CompositeValidationChainMember<T, TEntity> : IValidationChainMember<T>
  {
    private readonly IEnumerable<IValidationRuleSet<TEntity>> _contexts;

    private readonly IList<DependsOnMember> _dependOnRules = new List<DependsOnMember>();
    private readonly Func<T, IEnumerable<TEntity>> _func;
    private readonly string _ruleIdentity;

    private readonly IValidator _validator = new Validator();
    private readonly IValidatorAsync _validatorAsync = new ValidatorAsync();

    public CompositeValidationChainMember(
      IEnumerable<IValidationRuleSet<TEntity>> contexts, Expression<Func<T, IEnumerable<TEntity>>> expression)
    {
      if (expression == null)
      {
        throw new ArgumentNullException(nameof(expression));
      }

      _contexts = contexts ?? throw new ArgumentNullException(nameof(contexts));
      _func = expression.Compile();
      _ruleIdentity = Guid.NewGuid()
                          .ToString();
    }

    public ValidationResult Process(T value, ValidationContext context)
    {
      var result = new ValidationResult();
      IEnumerable<TEntity> enumerable;
      lock (_func)
      {
        enumerable = _func.Invoke(value);
      }
      lock (_contexts)
      {
        foreach (var res in enumerable.Select(item => _validator.ValidateMany(_contexts, item, context)))
        {
          result.Results.AddRange(res.Results);
        }
      }
      return result;
    }

    public async Task<ValidationResult> ProcessAsync(T value, ValidationContext context)
    {
      var result = new ValidationResult();
      IEnumerable<TEntity> enumerable;
      lock (_func)
      {
        enumerable = _func.Invoke(value);
      }

      foreach (var res in enumerable.Select(item => _validatorAsync.ValidateManyAsync(_contexts, item, context)))
      {
        var validateResult = await res;
        result.Results.AddRange(validateResult.Results);
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

    public RuleIdentity Identity => new RuleIdentity(_ruleIdentity);

    public IEnumerable<DependsOnMember> DependOnRules => _dependOnRules;
  }
}
