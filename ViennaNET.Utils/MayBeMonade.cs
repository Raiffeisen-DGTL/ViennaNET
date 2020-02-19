using System;
using System.Collections.Generic;

namespace ViennaNET.Utils
{
  /// <summary>
  /// Вспомогательные функции для определения возможности действий
  /// </summary>
  public static class MayBeMonade
  {
    /// <summary>
    /// Генерирует исключение в случае если выполняется указанное условие
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <typeparam name="TException">Тип исключения</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <param name="predicate">Ссылка на функцию, вычисляющую условие</param>
    /// <param name="exception">Ссылка на исключение</param>
    /// <returns>Ссылку на объект</returns>
    public static TInput ThrowIf<TInput, TException>(this TInput obj, Predicate<TInput> predicate, TException exception)
      where TInput : class where TException : Exception
    {
      if (obj == null)
      {
        return null;
      }

      if (predicate(obj))
      {
        throw exception;
      }

      return obj;
    }

    /// <summary>
    /// Генерирует исключение в случае если выполняется указанное условие
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="target">Ссылка на объект</param>
    /// <param name="predicate">Ссылка на функцию, вычисляющую условие</param>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="args">Параметры сообщения для форматирования</param>
    /// <returns>Ссылку на объект</returns>
    public static TInput ThrowIf<TInput>(this TInput target, Predicate<TInput> predicate, string message, params object[] args)
    {
      if (target == null)
      {
        return default;
      }

      if (predicate(target))
      {
        throw new Exception(string.Format(message, args));
      }

      return target;
    }

    /// <summary>
    /// Генерирует исключение в случае если выполняется указанное условие
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <typeparam name="TException">Тип исключения</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <param name="predicate">Ссылка на функцию, вычисляющую условие</param>
    /// <param name="throwExc">Ссылка на функцию, вызывающая исключение</param>
    /// <returns>Ссылку на объект</returns>
    public static TInput ThrowIf<TInput, TException>(this TInput obj, Predicate<TInput> predicate, Func<TInput, TException> throwExc)
      where TInput : class where TException : Exception
    {
      if (obj == null)
      {
        return null;
      }

      if (predicate(obj))
      {
        throw throwExc(obj);
      }

      return obj;
    }

    /// <summary>
    /// Генерирует исключение в случае если не выполняется указанное условие
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <typeparam name="TException">Тип исключения</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <param name="predicate">Ссылка на функцию, вычисляющую условие</param>
    /// <param name="exception">Ссылка на исключение</param>
    /// <returns>Ссылку на объект</returns>
    public static TInput ThrowIfNot<TInput, TException>(this TInput obj, Predicate<TInput> predicate, TException exception)
      where TInput : class where TException : Exception
    {
      if (obj == null)
      {
        return null;
      }

      if (!predicate(obj))
      {
        throw exception;
      }

      return obj;
    }

    /// <summary>
    /// Генерирует исключение в случае если не выполняется указанное условие
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <typeparam name="TException">Тип исключения</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <param name="predicate">Ссылка на функцию, вычисляющую условие</param>
    /// <param name="throwExc">Ссылка на функцию, вызывающая исключение</param>
    /// <returns>Ссылку на объект</returns>
    public static TInput ThrowIfNot<TInput, TException>(this TInput obj, Predicate<TInput> predicate, Func<TInput, TException> throwExc)
      where TInput : class where TException : Exception
    {
      if (obj == null)
      {
        return null;
      }

      if (!predicate(obj))
      {
        throw throwExc(obj);
      }

      return obj;
    }

    /// <summary>
    /// Вычисляет значение функции по элементу коллекции с указанным индексом
    /// </summary>
    /// <typeparam name="TInput">Тип элемента</typeparam>
    /// <typeparam name="TResult">Тип результата</typeparam>
    /// <param name="obj">Ссылка на коллекцию</param>
    /// <param name="index">Индекс объекта в коллекции</param>
    /// <param name="evaluator">Ссылка на функцию для расчета </param>
    /// <param name="failureValue">Результат в случае если индекс не существует</param>
    /// <returns>Результат расчета</returns>
    public static TResult At<TInput, TResult>(
      this IList<TInput> obj, int index, Func<TInput, TResult> evaluator, TResult failureValue = default) where TInput : class
    {
      return obj == null
        ? failureValue
        : obj.Count <= index
          ? failureValue
          : evaluator(obj[index]);
    }

    /// <summary>
    /// Выполняет действие, если ссылка на объект не пуста
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <param name="action">Действие для выполнения</param>
    /// <returns>Ссылка на объект</returns>
    public static TInput Do<TInput>(this TInput obj, Action<TInput> action) where TInput : class
    {
      if (obj == null)
      {
        return null;
      }

      action(obj);
      return obj;
    }

    /// <summary>
    /// Выполняет действие, если nullable-структура имеет значение
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="obj">Nullable-структура</param>
    /// <param name="action">Действие для выполнения</param>
    /// <returns>Nullable-структура</returns>
    public static TInput? Do<TInput>(this TInput? obj, Action<TInput?> action) where TInput : struct
    {
      if (obj == null)
      {
        return null;
      }

      action(obj);
      return obj;
    }

    /// <summary>
    /// Возвращает объект, если результат выполнения функции true 
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <param name="evaluator">Ссылка на функцию для расчета</param>
    /// <returns>Ссылка на объект</returns>
    public static TInput If<TInput>(this TInput obj, Func<TInput, bool> evaluator) where TInput : class
    {
      if (obj == null)
      {
        return null;
      }

      return evaluator(obj)
        ? obj
        : null;
    }

    /// <summary>
    /// Возвращает объект, если результат выполнения функции false
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <param name="evaluator">Ссылка на функцию для расчета</param>
    /// <returns>Ссылка на объект</returns>
    public static TInput IfNot<TInput>(this TInput obj, Func<TInput, bool> evaluator) where TInput : class
    {
      if (obj == null)
      {
        return null;
      }

      return evaluator(obj)
        ? null
        : obj;
    }

    /// <summary>
    /// Возвращает структуру, если результат выполнения функции true 
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="obj">Nullable-структура</param>
    /// <param name="evaluator">Ссылка на функцию для расчета</param>
    /// <returns>Nullable-структура</returns>
    public static TInput? If<TInput>(this TInput? obj, Func<TInput?, bool> evaluator) where TInput : struct
    {
      if (obj == null)
      {
        return null;
      }

      return evaluator(obj)
        ? obj
        : null;
    }

    /// <summary>
    /// Возвращает структуру, если результат выполнения функции false
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="obj">Nullable-структура</param>
    /// <param name="evaluator">Ссылка на функцию для расчета</param>
    /// <returns>Nullable-структура</returns>
    public static TInput? IfNot<TInput>(this TInput? obj, Func<TInput?, bool> evaluator) where TInput : struct
    {
      if (obj == null)
      {
        return null;
      }

      return evaluator(obj)
        ? null
        : obj;
    }

    /// <summary>
    /// Возвращает результат функции, если ссылка на объект не null
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <typeparam name="TResult">Тип результата</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <param name="evaluator">Ссылка на выполняемую функцию</param>
    /// <param name="failureValue">Результат в случае ссылка на объект равна null</param>
    /// <returns>Результат выполнения функции</returns>
    public static TResult Return<TInput, TResult>(
      this TInput obj, Func<TInput, TResult> evaluator, TResult failureValue = default) where TInput : class
    {
      return obj == null
        ? failureValue
        : evaluator(obj);
    }

    /// <summary>
    /// Возвращает результат функции, если nullable-структура не имеет значения
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <typeparam name="TResult">Тип результата</typeparam>
    /// <param name="obj">Nullable-структура</param>
    /// <param name="evaluator">Ссылка на выполняемую функцию</param>
    /// <param name="failureValue">Результат в случае если nullable-структура не имеет значения </param>
    /// <returns>Результат выполнения функции</returns>
    public static TResult Return<TInput, TResult>(
      this TInput? obj, Func<TInput?, TResult> evaluator, TResult failureValue = default) where TInput : struct
    {
      return obj.HasValue
        ? evaluator(obj)
        : failureValue;
    }

    /// <summary>
    /// Проверяет, что ссылка на объект не null
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <returns>true если не null, иначе false</returns>
    public static bool ReturnSuccess<TInput>(this TInput obj) where TInput : class
    {
      return obj != null;
    }

    /// <summary>
    /// Выполняет действие, если объект null
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="obj">Ссылка на объект</param>
    /// <param name="evaluator">Функция для выполнения</param>
    /// <returns>Ссылка на объект</returns>
    public static TInput IfNull<TInput>(this TInput obj, Action evaluator) where TInput : class
    {
      if (obj == null)
      {
        evaluator();
      }

      return obj;
    }

    /// <summary>
    /// Генерирует <see cref="ArgumentNullException"/> в случае если ссылка на объект равна null
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="param">Ссылка на объект</param>
    /// <param name="paramName">Имя параметра</param>
    /// <returns>Ссылка на объект</returns>
    [AssertionMethod]
    [NotNull]
    public static TInput ThrowIfNull<TInput>(
      [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this TInput param, [InvokerParameterName] string paramName = null)
      where TInput : class
    {
      if (param == null)
      {
        throw new ArgumentNullException(paramName ?? "param");
      }

      return param;
    }

    /// <summary>
    /// Генерирует <see cref="ArgumentNullException"/> в случае
    /// если строка не пуста, и ссылка на нее не равна null
    /// </summary>
    /// <param name="param">Ссылка на строку</param>
    /// <param name="paramName">Имя параметра</param>
    /// <returns>Ссылка на строку</returns>
    [AssertionMethod]
    [NotNull]
    public static string ThrowIfNullOrEmpty(
      [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this string param, [InvokerParameterName] string paramName = null)
    {
      if (string.IsNullOrEmpty(param))
      {
        throw new ArgumentNullException(paramName ?? "param");
      }
      return param;
    }

    /// <summary>
    /// Генерирует исключение в случае строка не пуста, и ссылка на нее не равна null
    /// </summary>
    /// <param name="param">Ссылка на строку</param>
    /// <param name="exception">Ссылка на исключение</param>
    /// <returns>Ссылка на строку</returns>
    public static string ThrowIfNullOrEmpty(this string param, Exception exception)
    {
      if (string.IsNullOrEmpty(param))
      {
        throw exception;
      }
      return param;
    }

    /// <summary>
    /// Генерирует <see cref="ArgumentNullException"/> в случае если строка не
    /// пуста и не содержит только пробелы, и ссылка на нее не равна null
    /// </summary>
    /// <param name="param">Ссылка на строку</param>
    /// <param name="paramName">Имя параметра</param>
    /// <returns>Ссылка на строку</returns>
    [AssertionMethod]
    [NotNull]
    public static string ThrowIfNullOrWhiteSpace(
      [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this string param, [InvokerParameterName] string paramName = null)
    {
      if (string.IsNullOrWhiteSpace(param))
      {
        throw new ArgumentNullException(paramName ?? "param");
      }
      return param;
    }

    /// <summary>
    /// Генерирует исключение в случае если строка не
    /// пуста и не содержит только пробелы, и ссылка на нее не равна null
    /// </summary>
    /// <param name="param">Ссылка на строку</param>
    /// <param name="exception">Ссылка на исключение</param>
    /// <returns>Ссылка на строку</returns>
    public static string ThrowIfNullOrWhiteSpace(this string param, Exception exception)
    {
      if (string.IsNullOrWhiteSpace(param))
      {
        throw exception;
      }
      return param;
    }

    /// <summary>
    /// Генерирует <see cref="ArgumentNullException"/> в случае если ссылка на объект равна null
    /// </summary>
    /// <typeparam name="TInput">Тип объекта</typeparam>
    /// <param name="param">Ссылка на объект</param>
    /// <param name="exception">Ссылка на исключение</param>
    /// <returns>Ссылка на объект</returns>
    public static TInput ThrowIfNull<TInput>(this TInput param, Exception exception) where TInput : class
    {
      if (param == null)
      {
        throw exception;
      }

      return param;
    }
  }
}
