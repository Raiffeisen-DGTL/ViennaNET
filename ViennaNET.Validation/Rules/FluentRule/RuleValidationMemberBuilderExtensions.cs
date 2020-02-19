using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ViennaNET.Validation.Rules.FluentRule.RuleValidators;

namespace ViennaNET.Validation.Rules.FluentRule
{
  /// <summary>
  /// Методы расширения для строителя валидаторов правила с текучим интерфейсом
  /// </summary>
  public static class RuleValidationMemberBuilderExtensions
  {
    /// <summary>
    /// Задает валидатор, проверяющий, что ссылка на свойство не содержит null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> NotNull<T, TProperty>(this RuleValidationMemberBuilder<T, TProperty> obj)
      where TProperty : class
    {
      return obj.SetValidator(new NotNullRuleValidator<TProperty>());
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что ссылка на свойство не содержит null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty?> NotNull<T, TProperty>(this RuleValidationMemberBuilder<T, TProperty?> obj)
      where TProperty : struct
    {
      return obj.SetValidator(new HasValueRuleValidator<TProperty>());
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что ссылка на свойство содержит null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> Null<T, TProperty>(this RuleValidationMemberBuilder<T, TProperty> obj)
      where TProperty : class
    {
      return obj.SetValidator(new NullRuleValidator<TProperty>());
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что ссылка на свойство содержит null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty?> Null<T, TProperty>(this RuleValidationMemberBuilder<T, TProperty?> obj)
      where TProperty : struct
    {
      return obj.SetValidator(new HasNoValueRuleValidator<TProperty>());
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что значение свойства равно указанному
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <param name="comparer">Компаратор</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> Equal<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, TProperty compareTo, IEqualityComparer<TProperty> comparer = null)
    {
      return obj.SetValidator(new EqualValidator<TProperty>(compareTo, comparer));
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что значение свойства больше указанного или равно ему
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> GreaterThanOrEqual<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, TProperty compareTo) where TProperty : IComparable
    {
      return obj.SetValidator(new GreaterThanOrEqualValidator<TProperty>(compareTo));
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что значение свойства больше указанного или равно ему
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty?> GreaterThanOrEqual<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty?> obj, TProperty compareTo)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return obj.SetValidator(new GreaterThanOrEqualValidator<TProperty>(compareTo));
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что значение свойства больше указанного
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> GreaterThan<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, TProperty compareTo) where TProperty : IComparable
    {
      return obj.SetValidator(new GreaterThanValidator<TProperty>(compareTo));
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что значение свойства больше указанного
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty?> GreaterThan<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty?> obj, TProperty compareTo)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return obj.SetValidator(new GreaterThanValidator<TProperty>(compareTo));
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что длина значения свойства лежит
    /// в указанных границах. Граничные значения длины тоже допустимы
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="min">Минимальная длина</param>
    /// <param name="max">Максимальная длина</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> Length<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, int min, int max)
    {
      return obj.SetValidator(new LengthValidator<TProperty>(min, max));
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что строка не пуста и ссылка на нее не равна null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, string> NotEmpty<T>(this RuleValidationMemberBuilder<T, string> obj)
    {
      return obj.SetValidator(new NotEmptyRuleValidator());
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что строка соответствует регулярному выражению
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="expression">Строка регулярного выражения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, string> Matches<T>(this RuleValidationMemberBuilder<T, string> obj, string expression)
    {
      return obj.SetValidator(new RegexRuleValidator(expression));
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что строка соответствует регулярному выражению
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="expression">Регулярное выражение</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, string> Matches<T>(this RuleValidationMemberBuilder<T, string> obj, Regex expression)
    {
      return obj.SetValidator(new RegexRuleValidator(expression));
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что свойство соответствует переданному критерию
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="criteria">Критерий соответствия</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> Must<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, Func<TProperty, object, bool> criteria)
    {
      return obj.SetValidator(new MustValidator<TProperty>(criteria));
    }

    /// <summary>
    /// Задает валидатор, проверяющий свойство правилом валидации.
    /// Подходит для валидации вложенных свойств
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="rule">Правило валидации</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> UseRule<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, IRule<TProperty> rule)
    {
      return obj.SetValidator(new UseIRulePropertyValidator<TProperty>(rule));
    }

    /// <summary>
    /// Задает условие, при котором правило будет выполняться
    /// Подходит для валидации вложенных свойств
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="criteria">Функция, возвращающее булево значение условия, при котором правило будет выполняться</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static RuleValidationMemberBuilder<T, TProperty> When<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, Func<T, object, bool> criteria)
    {
      return obj.ApplyCondition(criteria);
    }

    /// <summary>
    /// Задает валидатор, проверяющий, что значение свойства находится
    /// между заданными значениями. Границы включаются.
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="from">Нижнее пороговое значение</param>
    /// <param name="to">Верхнее пороговое значение</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> InclusiveBetween<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, IComparable from, IComparable to) where TProperty : IComparable
    {
      return obj.SetValidator(new InclusiveBetweenValidator<TProperty>(from, to));
    }
  }
}