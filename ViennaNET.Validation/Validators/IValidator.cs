using System.Collections.Generic;
using ViennaNET.Validation.Rules;

namespace ViennaNET.Validation.Validators
{
  /// <summary>
  /// Синхронный интерфейс валидатора
  /// </summary>
  public interface IValidator
  {
    /// <summary>
    /// Валидирует заданный объект набором правил валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="ruleSet">Набор правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    ValidationResult Validate<T>(IValidationRuleSet<T> ruleSet, T instance, ValidationContext context);

    /// <summary>
    /// Валидирует заданный объект несколькими наборами правил валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="ruleSets">Коллекция наборов правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    ValidationResult ValidateMany<T>(IEnumerable<IValidationRuleSet<T>> ruleSets, T instance, ValidationContext context);

    /// <summary>
    /// Валидирует заданный объект коллекцией правил валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="rules">Коллекция правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    ValidationResult Validate<T>(IEnumerable<IRule<T>> rules, T instance, ValidationContext context);

    /// <summary>
    /// Валидирует заданный объект правилом валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="rule">Правило валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    ValidationResult Validate<T>(IRule<T> rule, T instance, ValidationContext context);
  }
}
