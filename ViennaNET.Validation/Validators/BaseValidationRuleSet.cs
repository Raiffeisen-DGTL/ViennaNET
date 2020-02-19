using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;
using ViennaNET.Validation.ValidationChains;

namespace ViennaNET.Validation.Validators
{
  /// <summary>
  /// Базовый класс для создания набора правил
  /// </summary>
  /// <typeparam name="T">Тип объекта для валидации</typeparam>
  public abstract class BaseValidationRuleSet<T> : IValidationRuleSet<T>
  {
    private readonly ValidationChain<IValidationChainMember<T>> _validationChain = new ValidationChain<IValidationChainMember<T>>();

    /// <inheritdoc />
    public IValidationChainMember<T> AddMemberToChain(IRuleBase<T> rule)
    {
      if (rule == null)
      {
        throw new ArgumentNullException(nameof(rule));
      }

      var member = _validationChain.FirstOrDefault(x => x.Identity == rule.Identity);
      if (member != null)
      {
        return member;
      }

      member = new ValidationChainMember<T>(rule);
      AddMember(member);
      return member;
    }

    /// <inheritdoc />
    IList<IValidationChainMember<T>> IValidationRuleSet<T>.GetValidationChain()
    {
      return _validationChain.Clone();
    }

    protected IValidationChainMemberBuilder<T> SetRule(IRuleBase<T> rule)
    {
      var member = AddMemberToChain(rule);
      return new ValidationChainMemberBuilder<T>(this, member);
    }

    private void AddMember(IValidationChainMember<T> member)
    {
      _validationChain.Add(member);
    }

    protected void When(IRuleBase<T> rule, Action action)
    {
      WhenInternal(rule, action, null);
    }

    private void WhenInternal(IRuleBase<T> rule, Action action, Func<ValidationResult, bool> condition)
    {
      var member = AddMemberToChain(rule);
      var list = new List<IValidationChainMember<T>>();
      using (_validationChain.OnItemAdded((r, e) => list.Add(e.Value)))
      {
        action();
      }
      list.ForEach(x =>
      {
        if (condition == null)
        {
          x.DependsOn(member);
        }
        else
        {
          x.DependsOn(member, condition);
        }
      });
    }

    protected void WhenNoWarnings(IRuleBase<T> rule, Action action)
    {
      WhenInternal(rule, action, r => !r.Results.Any(x => x.Messages.Any(m => m is WarningRuleMessage)));
    }

    protected void WhenNoWarningsAndErrors(IRuleBase<T> rule, Action action)
    {
      WhenInternal(rule, action, r => !r.Results.Any(x => x.Messages.Any(m => m is WarningRuleMessage || m is ErrorRuleMessage)));
    }

    protected void WhenNoInfos(IRuleBase<T> rule, Action action)
    {
      WhenInternal(rule, action, r => !r.Results.Any(x => x.Messages.Any(m => m is InfoRuleMessage)));
    }

    protected void WhenNoInfosAndWarnings(IRuleBase<T> rule, Action action)
    {
      WhenInternal(rule, action, r => !r.Results.Any(x => x.Messages.Any(m => m is WarningRuleMessage || m is InfoRuleMessage)));
    }

    protected void WhenNoInfosAndErrors(IRuleBase<T> rule, Action action)
    {
      WhenInternal(rule, action, r => !r.Results.Any(x => x.Messages.Any(m => m is ErrorRuleMessage || m is InfoRuleMessage)));
    }

    protected IValidationChainMemberBuilder<T> SetCollectionContext<TEntity>(
      Expression<Func<T, IEnumerable<TEntity>>> expression, IValidationRuleSet<TEntity> ruleSet)
    {
      var member = AddCollectionMemberToChain(expression, new[] { ruleSet });
      return new ValidationChainMemberBuilder<T>(this, member);
    }

    protected IValidationChainMemberBuilder<T> SetCollectionContext<TEntity>(
      Expression<Func<T, IEnumerable<TEntity>>> expression, IEnumerable<IValidationRuleSet<TEntity>> contexts)
    {
      var member = AddCollectionMemberToChain(expression, contexts);
      return new ValidationChainMemberBuilder<T>(this, member);
    }

    private IValidationChainMember<T> AddCollectionMemberToChain<TEntity>(
      Expression<Func<T, IEnumerable<TEntity>>> expression, IEnumerable<IValidationRuleSet<TEntity>> contexts)
    {
      if (expression == null)
      {
        throw new ArgumentNullException(nameof(expression));
      }

      var member = new CompositeValidationChainMember<T, TEntity>(contexts, expression);
      AddMember(member);
      return member;
    }

    /// <summary>
    /// Задает для участника цепи логику, позволяющую останавливать выполнение цепи
    /// </summary>
    /// <param name="member">Участник сообщений</param>
    /// <returns>Участник цепи</returns>
    internal IValidationChainMember<T> SetStopOnFailure(IValidationChainMember<T> member)
    {
      var decorator = new StopOnFailureChainMemberDecorator<T>(member);
      _validationChain.Remove(member);
      AddMember(decorator);
      return decorator;
    }
  }
}
