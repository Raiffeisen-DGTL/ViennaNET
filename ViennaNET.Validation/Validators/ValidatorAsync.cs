using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.ValidationChains;
using ViennaNET.Validation.Validators.Exceptions;

namespace ViennaNET.Validation.Validators
{
  /// <summary>
  /// Синхронный валидатор. Обеспечивает выполнение цепи валидации,
  /// учитывая правила, заданные для различных участников цепи
  /// </summary>
  public class ValidatorAsync : IValidatorAsync
  {
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync<T>(IValidationRuleSet<T> ruleSet, T instance, ValidationContext context)
    {
      if (ruleSet == null)
      {
        throw new ArgumentNullException(nameof(ruleSet));
      }

      if (instance == null)
      {
        throw new ArgumentNullException(nameof(instance));
      }

      return ValidateInternal(ruleSet, instance, context);
    }

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateManyAsync<T>(IEnumerable<IValidationRuleSet<T>> ruleSets, T instance, ValidationContext context)
    {
      if (ruleSets == null)
      {
        throw new ArgumentNullException(nameof(ruleSets));
      }

      if (instance == null)
      {
        throw new ArgumentNullException(nameof(instance));
      }

      var result = new ValidationResult();
      foreach (var ruleSet in ruleSets)
      {
        var res = await ValidateInternal(ruleSet, instance, context);
        result.Results.AddRange(res.Results);
      }
      return result;
    }

    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync<T>(IEnumerable<IRuleBase<T>> rules, T instance, ValidationContext context)
    {
      if (rules == null)
      {
        throw new ArgumentNullException(nameof(rules));
      }

      if (instance == null)
      {
        throw new ArgumentNullException(nameof(instance));
      }

      var ruleSet = new SimpleValidationRuleSet<T>(rules);
      return ValidateInternal(ruleSet, instance, context);
    }

    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync<T>(IRuleBase<T> rule, T instance, ValidationContext context)
    {
      if (rule == null)
      {
        throw new ArgumentNullException(nameof(rule));
      }

      if (instance == null)
      {
        throw new ArgumentNullException(nameof(instance));
      }

      var ruleSet = new SimpleValidationRuleSet<T>(rule);
      return ValidateInternal(ruleSet, instance, context);
    }

    private static async Task<ValidationResult> ProcessRule<T>(
      T instance, IValidationChainMember<T> validationChainMember, Dictionary<RuleIdentity, ValidationResult> validationMap,
      ValidationResult result, ValidationContext context)
    {
      if (validationMap.TryGetValue(validationChainMember.Identity, out var validationResult))
      {
        return validationResult;
      }
      var res = await validationChainMember.ProcessAsync(instance, context);
      validationMap[validationChainMember.Identity] = res;
      result.MergeResult(res);
      return res;
    }

    private static async Task<bool> CheckDependsOnMembers<T>(
      IList<IValidationChainMember<T>> chain, T instance, IValidationChainMember<T> validationChainMember,
      Dictionary<RuleIdentity, ValidationResult> validationMap, ValidationResult result, ValidationContext context)
    {
      foreach (var dependsOnMember in validationChainMember.DependOnRules)
      {
        if (!validationMap.TryGetValue(dependsOnMember.RuleIdentity, out var savedResult))
        {
          var onMember = dependsOnMember;
          var member = chain.FirstOrDefault(x => x.Identity == onMember.RuleIdentity);
          if (member == null)
          {
            throw new ValidationException($"Не зарегистрировано правила с кодом {onMember.RuleIdentity.Code}");
          }
          if (!await CheckDependsOnMembers(chain, instance, member, validationMap, result, context))
          {
            return false;
          }
          var processResult = await ProcessRule(instance, member, validationMap, result, context);
          if (!dependsOnMember.SatisfyCondition(processResult))
          {
            return false;
          }
        }
        else
        {
          if (!dependsOnMember.SatisfyCondition(savedResult))
          {
            return false;
          }
        }
      }
      return true;
    }

    private static async Task<ValidationResult> ValidateInternal<T>(IValidationRuleSet<T> ruleSet, T instance, ValidationContext context)
    {
      var validationMap = new Dictionary<RuleIdentity, ValidationResult>();
      var result = new ValidationResult();
      var chain = ruleSet.GetValidationChain();
      try
      {
        foreach (var validationChainMember in chain)
        {
          if (await CheckDependsOnMembers(chain, instance, validationChainMember, validationMap, result, context))
          {
            await ProcessRule(instance, validationChainMember, validationMap, result, context);
          }
        }
      }
      catch (AggregateException ex) when (ex.InnerException is ValidationStoppedException valException)
      {
        result.MergeResult(valException.Result);
      }
      catch (ValidationStoppedException ex)
      {
        result.MergeResult(ex.Result);
      }

      return result;
    }
  }
}