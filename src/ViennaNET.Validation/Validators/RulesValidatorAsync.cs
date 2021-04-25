using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules;

namespace ViennaNET.Validation.Validators
{
  /// <summary>
  /// Статический класс для работы с валидатором
  /// </summary>
  public class RulesValidatorAsync
  {
    private static readonly IValidatorAsync validator = new ValidatorAsync();

    static RulesValidatorAsync()
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
    public static Task<ValidationResult> ValidateAsync<T>(IValidationRuleSet<T> ruleSet, T instance, ValidationContext context = null)
    {
      return validator.ValidateAsync(ruleSet, instance, context);
    }

    /// <summary>
    /// Валидирует заданный объект несколькими наборами правил валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="ruleSets">Коллекция наборов правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    public static Task<ValidationResult> ValidateManyAsync<T>(
      IEnumerable<IValidationRuleSet<T>> ruleSets, T instance, ValidationContext context = null)
    {
      return validator.ValidateManyAsync(ruleSets, instance, context);
    }

    /// <summary>
    /// Валидирует заданный объект коллекцией правил валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="rules">Коллекция правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    public static Task<ValidationResult> ValidateAsync<T>(IEnumerable<IRule<T>> rules, T instance, ValidationContext context = null)
    {
      return validator.ValidateAsync(rules, instance, context);
    }

    /// <summary>
    /// Валидирует заданный объект правилом валидации 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="rule">Правило валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    public static Task<ValidationResult> ValidateAsync<T>(IRule<T> rule, T instance, ValidationContext context = null)
    {
      return validator.ValidateAsync(rule, instance, context);
    }
  }
}