using System;
using System.Threading.Tasks;

namespace ViennaNET.Sagas
{
  /// <summary>
  ///   Описывает шаг, для его конфигурирования
  /// </summary>
  /// <typeparam name="TCont">Контекст саги</typeparam>
  public interface IConfigurableAsyncSagaStep<out TCont> where TCont : class
  {
    /// <summary>
    ///   Устанавливает основное действие
    /// </summary>
    /// <param name="action">Основное действие</param>
    /// <returns>Текущий шаг</returns>
    IConfigurableAsyncSagaStep<TCont> WithAction(Func<TCont, Task> action);

    /// <summary>
    ///   Устанавливает функцию компенсации основного действия
    /// </summary>
    /// <param name="compensation">Функция компенсации основного действия</param>
    /// <returns>Текущий шаг</returns>
    IConfigurableAsyncSagaStep<TCont> WithCompensation(Func<TCont, Task> compensation);
  }
}