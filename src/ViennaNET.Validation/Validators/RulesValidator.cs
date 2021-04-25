using System.Collections.Generic;
using ViennaNET.Validation.Rules;

namespace ViennaNET.Validation.Validators
{
  /// <summary>
  /// Статический класс для работы с валидатором
  /// </summary>
  public static class RulesValidator
  {
    private static readonly IValidator validator = new Validator();

    static RulesValidator()
    {
    }

    /// <summary>
    /// Валидирует заданный объект набором правил валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="ruleSet">Набор правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    public static ValidationResult Validate<T>(IValidationRuleSet<T> ruleSet, T instance, ValidationContext context = null)
    {
      return validator.Validate(ruleSet, instance, context);
    }

    /// <summary>
    /// Валидирует заданный объект несколькими наборами правил валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="ruleSets">Коллекция наборов правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    public static ValidationResult ValidateMany<T>(
      IEnumerable<IValidationRuleSet<T>> ruleSets, T instance, ValidationContext context = null)
    {
      return validator.ValidateMany(ruleSets, instance, context);
    }

    /// <summary>
    /// Валидирует заданный объект коллекцией правил валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="rules">Коллекция правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    public static ValidationResult Validate<T>(IEnumerable<IRule<T>> rules, T instance, ValidationContext context = null)
    {
      return validator.Validate(rules, instance, context);
    }

    /// <summary>
    /// Валидирует заданный объект правилом валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="rule">Правило валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    public static ValidationResult Validate<T>(IRule<T> rule, T instance, ValidationContext context = null)
    {
      return validator.Validate(rule, instance, context);
    }
  }
}
