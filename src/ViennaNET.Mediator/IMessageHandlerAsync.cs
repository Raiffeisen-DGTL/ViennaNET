using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator
{
  /// <summary>
  /// Интерфейс-маркер обработчика сообщений
  /// </summary>
  public interface IMessageHandlerAsync
  {
  }

  /// <summary>
  /// Типизированный интерфейс-маркер обработчика сообщений
  /// </summary>
  /// <typeparam name="TMessage">Type of the message.</typeparam>
  public interface IMessageHandlerAsync<in TMessage> : IMessageHandlerAsync
    where TMessage : class, IMessage
  {
    /// <summary>
    /// Асинхронно обрабатывает сообщение, не возвращая результатов
    /// </summary>
    /// <param name="message">Ссылка на сообщение.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken);
  }

  /// <summary>
  /// Типизированный интерфейс-маркер обработчика запросов, предполагающего ответ
  /// </summary>
  /// <typeparam name="TMessage">Тип сообщения.</typeparam>
  /// <typeparam name="TResponse">Тип результата.</typeparam>
  public interface IMessageHandlerAsync<in TMessage, TResponse> : IMessageHandlerAsync
    where TMessage : class, IMessage
  {
    /// <summary>
    /// Асинхронно обрабатывает сообщение, возвращая результат.
    /// </summary>
    /// <param name="message">Ссылка на сообщение.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Результат обработки.</returns>
    Task<TResponse> HandleAsync(TMessage message, CancellationToken cancellationToken);
  }
}
