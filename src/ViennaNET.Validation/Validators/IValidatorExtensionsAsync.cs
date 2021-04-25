using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators.Exceptions;

namespace ViennaNET.Validation.Validators
{
  /// <summary>
  /// Методы расширения для интерфейса валидатора
  /// </summary>
  public static class IValidatorExtensionsAsync
  {
    /// <summary>
    /// Валидирует объект набором правил и генерирует исключение,
    /// если результат содержит ошибки
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <param name="validator">Валидатор</param>
    /// <param name="ruleSet">Набор правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    public static Task ValidateAndThrowAsync<T>(
      this IValidatorAsync validator, IValidationRuleSet<T> ruleSet, T instance, ValidationContext context)
    {
      var result = validator.ValidateAsync(ruleSet, instance, context);
      return ThrowIfIsNotValid(result);
    }

    /// <summary>
    /// Валидирует объект спинабором правил и генерирует исключение,
    /// если результат содержит ошибки 
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <param name="validator">Валидатор</param>
    /// <param name="ruleSets">Коллекция наборов правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    public static Task ValidateManyAndThrowAsync<T>(
      this IValidatorAsync validator, IEnumerable<IValidationRuleSet<T>> ruleSets, T instance, ValidationContext context)
    {
      var result = validator.ValidateManyAsync(ruleSets, instance, context);
      return ThrowIfIsNotValid(result);
    }

    /// <summary>
    /// Валидирует заданный объект коллекцией правил валидации
    /// и генерирует исключение, если результат содержит ошибки 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="validator">Валидатор</param>
    /// <param name="rules">Коллекция правил валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    public static Task ValidateAndThrow<T>(
      this IValidatorAsync validator, IEnumerable<IRule<T>> rules, T instance, ValidationContext context)
    {
      var result = validator.ValidateAsync(rules, instance, context);
      return ThrowIfIsNotValid(result);
    }

    /// <summary>
    /// Валидирует заданный объект правилом валидации 
    /// и генерирует исключение, если результат содержит ошибки 
    /// </summary>
    /// <typeparam name="T">Тип объекта для валидации</typeparam>
    /// <param name="validator">Валидатор</param>
    /// <param name="rule">Правило валидации</param>
    /// <param name="instance">Ссылка на объект валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации</returns>
    public static Task ValidateAndThrow<T>(this IValidatorAsync validator, IRule<T> rule, T instance, ValidationContext context)
    {
      var result = validator.ValidateAsync(rule, instance, context);
      return ThrowIfIsNotValid(result);
    }

    private static async Task ThrowIfIsNotValid(Task<ValidationResult> result)
    {
      var validateResult = await result;
      if (validateResult.IsValid)
      {
        return;
      }

      var errorMessage = string.Join(Environment.NewLine, validateResult.Results.Where(r => !r.IsValid)
                                                                        .Select(r => string.Join(Environment.NewLine,
                                                                                                 r.Messages.Select(m => m.Error))));

      throw new ValidationException(errorMessage);
    }
  }
}
