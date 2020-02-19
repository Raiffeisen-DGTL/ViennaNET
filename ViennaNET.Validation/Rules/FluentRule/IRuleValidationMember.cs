using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules.FluentRule.RuleValidators;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.FluentRule
{
  /// <summary>
  /// Участник цепи валидаторов правила с текучим интерфейсом
  /// </summary>
  /// <typeparam name="T">Тип объекта для валидации</typeparam>
  public interface IRuleValidationMember<in T>
  {
    /// <summary>
    /// Синхронно запускает цепь валидаторов
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Набор сообщений правила</returns>
    IEnumerable<IRuleMessage> Validate(T instance, ValidationContext context);

    /// <summary>
    /// Асинхронно запускает цепь валидаторов
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Набор сообщений правила</returns>
    Task<IEnumerable<IRuleMessage>> ValidateAsync(T instance, ValidationContext context);

    /// <summary>
    /// Коллекция валидаторов правила
    /// </summary>
    IEnumerable<IRuleValidatorBase> Validators { get; }

    /// <summary>
    /// Устанавливает условие выполнение валидатора
    /// </summary>
    /// <param name="condition">Условие выполнения валидатора</param>
    /// <returns>Ссылка на строитель</returns>
    void ApplyCondition(Func<object, object, bool> condition);
    void ApplyCondition(Func<object, object, Task<bool>> condition);

    /// <summary>
    /// Устанавливает валидатор правила с текучим интерфейсом
    /// </summary>
    /// <param name="validator">Валидатор правила с текучим интерфейсом</param>
    /// <returns>Ссылка на строитель</returns>
    void AddValidator(IRuleValidatorBase validator);
  }
}
