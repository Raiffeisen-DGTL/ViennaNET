using System;

namespace ViennaNET.Sagas
{
  /// <summary>
  /// Описывает шаг, для его конфигурирования
  /// </summary>
  /// <typeparam name="TCont">Контекст саги</typeparam>
  public interface IConfigurableSagaStep<out TCont> where TCont : class
  {
    /// <summary>
    /// Устанавливает основное действие
    /// </summary>
    /// <param name="action">Основное действие</param>
    /// <returns>Текущий шаг</returns>
    IConfigurableSagaStep<TCont> WithAction(Action<TCont> action);

    /// <summary>
    /// Устанавливает функцию компенсации основного действия
    /// </summary>
    /// <param name="compensation">Функция компенсации основного действия</param>
    /// <returns>Текущий шаг</returns>
    IConfigurableSagaStep<TCont> WithCompensation(Action<TCont> compensation);
  }
}
