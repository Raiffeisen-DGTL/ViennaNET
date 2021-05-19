using System;
using System.Collections.Generic;
using System.Linq;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators.Exceptions;

namespace ViennaNET.Validation.Validators
{
  /// <summary>
  /// Методы расширения для интерфейса валидатора
  /// </summary>
  public static class IValidatorExtensions
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
    public static void ValidateAndThrow<T>(
      this IValidator validator, IValidationRuleSet<T> ruleSet, T instance, ValidationContext context)
    {
      var result = validator.Validate(ruleSet, instance, context);
      ThrowIfIsNotValid(result);
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
    public static void ValidateManyAndThrow<T>(
      this IValidator validator, IEnumerable<IValidationRuleSet<T>> ruleSets, T instance, ValidationContext context)
    {
      var result = validator.ValidateMany(ruleSets, instance, context);
      ThrowIfIsNotValid(result);
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
    public static void ValidateAndThrow<T>(this IValidator validator, IEnumerable<IRule<T>> rules, T instance, ValidationContext context)
    {
      var result = validator.Validate(rules, instance, context);
      ThrowIfIsNotValid(result);
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
    public static void ValidateAndThrow<T>(this IValidator validator, IRule<T> rule, T instance, ValidationContext context)
    {
      var result = validator.Validate(rule, instance, context);
      ThrowIfIsNotValid(result);
    }

    private static void ThrowIfIsNotValid(ValidationResult result)
    {
      if (result.IsValid)
      {
        return;
      }

      var errorMessage = string.Join(Environment.NewLine, result.Results.Where(r => !r.IsValid)
                                                                .Select(r => string.Join(Environment.NewLine,
                                                                                         r.Messages.Select(m => m.Error))));

      throw new ValidationException(errorMessage);
    }
  }
}
