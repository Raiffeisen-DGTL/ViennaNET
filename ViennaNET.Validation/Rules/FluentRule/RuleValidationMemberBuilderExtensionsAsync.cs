using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules.FluentRule.RuleValidators;

namespace ViennaNET.Validation.Rules.FluentRule
{
  /// <summary>
  ///   Методы расширения для строителя валидаторов правила с текучим интерфейсом
  /// </summary>
  public static class RuleValidationMemberBuilderExtensionsAsync
  {
    /// <summary>
    ///   Задает валидатор, проверяющий, что ссылка на свойство не содержит null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<TProperty>> NotNull<T, TProperty>(
      this RuleValidationMemberBuilder<T, Task<TProperty>> obj) where TProperty : class
    {
      return obj.SetValidator(new NotNullRuleValidatorAsync<TProperty>());
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что ссылка на свойство не содержит null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<TProperty?>> NotNull<T, TProperty>(
      this RuleValidationMemberBuilder<T, Task<TProperty?>> obj) where TProperty : struct
    {
      return obj.SetValidator(new HasValueRuleValidatorAsync<TProperty>());
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что ссылка на свойство содержит null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<TProperty>> Null<T, TProperty>(this RuleValidationMemberBuilder<T, Task<TProperty>> obj)
      where TProperty : class
    {
      return obj.SetValidator(new NullRuleValidatorAsync<TProperty>());
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что ссылка на свойство содержит null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<TProperty?>> Null<T, TProperty>(
      this RuleValidationMemberBuilder<T, Task<TProperty?>> obj) where TProperty : struct
    {
      return obj.SetValidator(new HasNoValueRuleValidatorAsync<TProperty>());
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что значение свойства равно указанному
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <param name="comparer">Компаратор</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> Equal<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, Task<TProperty> compareTo, IEqualityComparer<TProperty> comparer = null)
    {
      return obj.SetValidator(new EqualValidatorAsync<TProperty>(compareTo, comparer));
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что значение свойства больше указанного или равно ему
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> GreaterThanOrEqual<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, Task<TProperty> compareTo) where TProperty : IComparable
    {
      return obj.SetValidator(new GreaterThanOrEqualValidatorAsync<TProperty>(compareTo));
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что значение свойства больше указанного или равно ему
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty?> GreaterThanOrEqual<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty?> obj, Task<TProperty> compareTo)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return obj.SetValidator(new GreaterThanOrEqualValidatorAsync<TProperty>(compareTo));
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что значение свойства больше указанного
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> GreaterThan<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, Task<TProperty> compareTo) where TProperty : IComparable
    {
      return obj.SetValidator(new GreaterThanValidatorAsync<TProperty>(compareTo));
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что значение свойства больше указанного
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="compareTo">Значение для сравнения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty?> GreaterThan<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty?> obj, Task<TProperty> compareTo)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return obj.SetValidator(new GreaterThanValidatorAsync<TProperty>(compareTo));
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что длина значения свойства лежит
    ///   в указанных границах. Граничные значения длины тоже допустимы
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип валидируемого свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="min">Минимальная длина</param>
    /// <param name="max">Максимальная длина</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<TProperty>> Length<T, TProperty>(
      this RuleValidationMemberBuilder<T, Task<TProperty>> obj, int min, int max)
    {
      return obj.SetValidator(new LengthValidatorAsync<TProperty>(min, max));
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что строка не пуста и ссылка на нее не равна null
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<string>> NotEmpty<T>(this RuleValidationMemberBuilder<T, Task<string>> obj)
    {
      return obj.SetValidator(new NotEmptyRuleValidatorAsync());
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что строка соответствует регулярному выражению
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="expression">Строка регулярного выражения</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<string>> Matches<T>(
      this RuleValidationMemberBuilder<T, Task<string>> obj, string expression)
    {
      return obj.SetValidator(new RegexRuleValidatorAsync(expression));
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что строка соответствует регулярному выражению
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="expression">Регулярное выражение</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<string>> Matches<T>(
      this RuleValidationMemberBuilder<T, Task<string>> obj, Regex expression)
    {
      return obj.SetValidator(new RegexRuleValidatorAsync(expression));
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что свойство соответствует переданному критерию
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="criteria">Критерий соответствия</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<TProperty>> Must<T, TProperty>(
      this RuleValidationMemberBuilder<T, Task<TProperty>> obj, Func<Task<TProperty>, object, Task<bool>> criteria)
    {
      return obj.SetValidator(new MustValidatorAsync<TProperty>(criteria));
    }

    /// <summary>
    ///   Задает валидатор, проверяющий свойство правилом валидации.
    ///   Подходит для валидации вложенных свойств
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="rule">Правило валидации</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, TProperty> UseRuleAsync<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, IRuleAsync<TProperty> rule)
    {
      return obj.SetValidator(new UseIRulePropertyValidatorAsync<TProperty>(rule));
    }

    /// <summary>
    ///   Задает условие, при котором правило будет выполняться
    ///   Подходит для валидации вложенных свойств
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="criteria">Функция, возвращающее булево значение условия, при котором правило будет выполняться</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static RuleValidationMemberBuilder<T, TProperty> When<T, TProperty>(
      this RuleValidationMemberBuilder<T, TProperty> obj, Func<T, object, Task<bool>> criteria)
    {
      return obj.ApplyCondition(criteria);
    }

    /// <summary>
    ///   Задает валидатор, проверяющий, что значение свойства находится
    ///   между заданными значениями. Границы включаются.
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Строитель цепи валидаторов правила с текучим интерфейсом</param>
    /// <param name="from">Нижнее пороговое значение</param>
    /// <param name="to">Верхнее пороговое значение</param>
    /// <returns>Контейнер со ссылкой на последний валидатор в цепи</returns>
    public static CurrentValidatorHolder<T, Task<TProperty>> InclusiveBetween<T, TProperty>(
      this RuleValidationMemberBuilder<T, Task<TProperty>> obj, IComparable from, IComparable to) where TProperty : IComparable
    {
      return obj.SetValidator(new InclusiveBetweenValidatorAsync<TProperty>(from, to));
    }
  }
}
