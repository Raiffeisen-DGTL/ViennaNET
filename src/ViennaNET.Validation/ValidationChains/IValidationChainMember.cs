using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.ValidationChains
{
  /// <summary>
  /// Интерфейс участника валидационной цепи
  /// </summary>
  /// <typeparam name="T">Тип объекта участника</typeparam>
  public interface IValidationChainMember<T>
  {
    /// <summary>
    /// Идентификатор правила участника
    /// </summary>
    RuleIdentity Identity { get; }

    /// <summary>
    /// Коллекция зависимых правил валидации
    /// </summary>
    IEnumerable<DependsOnMember> DependOnRules { get; }

    /// <summary>
    /// Синхронно выполняет процесс валидации
    /// </summary>
    /// <param name="value">Объект для валидации</param>
    /// <param name="context">Контекст валидации</param>
    ValidationResult Process(T value, ValidationContext context);

    /// <summary>
    /// Асинхронно выполняет процесс валидации
    /// </summary>
    /// <param name="value">Объект для валидации</param>
    /// <param name="context">Контекст валидации</param>
    Task<ValidationResult> ProcessAsync(T value, ValidationContext context);

    /// <summary>
    /// Задает зависимость выполнения участника цепи от другого участника
    /// </summary>
    /// <param name="member">Участник, результат которого определяет запуск данного участника</param>
    void DependsOn(IValidationChainMember<T> member);

    /// <summary>
    /// Задает зависимость выполнения участника цепи от другого участника
    /// </summary>
    /// <param name="member">Участник цепи</param>
    /// <param name="condition">Условие, определяющее запуск другого участника цепи</param>
    void DependsOn(IValidationChainMember<T> member, Func<ValidationResult, bool> condition);
  }
}
