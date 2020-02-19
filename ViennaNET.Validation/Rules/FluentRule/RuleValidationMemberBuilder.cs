using System;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules.FluentRule.RuleValidators;

namespace ViennaNET.Validation.Rules.FluentRule
{
  /// <summary>
  /// Строитель валидаторов правила с текучим интерфейсом
  /// </summary>
  /// <typeparam name="T">Тип объекта для валидации</typeparam>
  /// <typeparam name="TProperty">Тип свойства</typeparam>
  public sealed class RuleValidationMemberBuilder<T, TProperty> : CurrentValidatorHolder<T, TProperty>
  {
    private readonly IRuleValidationMember<T> _member;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на участника цепи валидаторов правила с текучим интерфейсом
    /// </summary>
    /// <param name="member">Участник цепи валидаторов правила с текучим интерфейсом</param>
    public RuleValidationMemberBuilder(IRuleValidationMember<T> member)
    {
      _member = member;
    }

    /// <inheritdoc />
    internal override IRuleValidatorBase CurrentValidator { get; set; }

    /// <summary>
    /// Устанавливает валидатор правила с текучим интерфейсом
    /// </summary>
    /// <param name="validator">Валидатор правила с текучим интерфейсом</param>
    /// <returns>Ссылка на строитель</returns>
    public RuleValidationMemberBuilder<T, TProperty> SetValidator(IRuleValidatorBase validator)
    {
      if (validator == null)
      {
        throw new ArgumentNullException(nameof(validator));
      }

      _member.AddValidator(validator);
      CurrentValidator = validator;
      return this;
    }

    /// <summary>
    /// Устанавливает условие выполнение валидатора
    /// </summary>
    /// <param name="condition">Условие выполнения валидатора</param>
    /// <returns>Ссылка на строитель</returns>
    internal RuleValidationMemberBuilder<T, TProperty> ApplyCondition(Func<T, object, bool> condition)
    {
      if (condition == null)
      {
        throw new ArgumentNullException(nameof(condition));
      }

      _member.ApplyCondition((x, c) => condition((T)x, c));
      return this;
    }

    /// <summary>
    /// Устанавливает условие выполнение валидатора, возвращающее задачу
    /// </summary>
    /// <param name="condition">Условие выполнения валидатора</param>
    /// <returns>Ссылка на строитель</returns>
    internal RuleValidationMemberBuilder<T, TProperty> ApplyCondition(Func<T, object, Task<bool>> condition)
    {
      if (condition == null)
      {
        throw new ArgumentNullException(nameof(condition));
      }

      _member.ApplyCondition((x, c) => condition((T)x, c));
      return this;
    }
  }
}