using System;

namespace ViennaNET.Utils.Threading
{
  /// <summary>
  ///   Интерфейс стратегии для асинхронной обработки сообщений
  /// </summary>
  public interface IAsyncProcessingStrategy
  {
    /// <summary>
    ///   Обработать сообщение
    /// </summary>
    /// <param name="action">Действие</param>
    /// <param name="errorHandler">Обработчик ошибок в рамках действия</param>
    void ProcessAsync(Action action, Action<Exception> errorHandler);
  }
}