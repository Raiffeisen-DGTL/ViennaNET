using System;
using System.Collections.Generic;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{ 
  /// <summary>
  /// Базовый интерфейс валидатора
  /// </summary>
  public interface IRuleValidatorBase
  {
    /// <summary>
    /// Добавляет функции для расчета валидационных аргументов 
    /// </summary>
    /// <param name="func">Функции для отложенного расчета</param>
    void AddArguments(IEnumerable<Func<object, object>> func);

    /// <summary>
    /// Задает функцию отложенного получения сообщения
    /// Отложенное получение позволяет добавлять в текст сообщения
    /// данные из валидируемого объекта во время выполнения валидации
    /// </summary>
    /// <param name="message">Функция для отложенного получения сообщения</param>
    void SetMessageSource(Func<IRuleMessage> message);

    /// <summary>
    /// Задает функцию отложенного получения состояния сообщения
    /// </summary>
    /// <param name="state">Функция для отложенного получения состояния сообщения</param>
    void SetState(Func<object, object> state);
  }
}
