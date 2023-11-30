using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator
{
  /// <summary>
  ///   Определяет интерфейс медиатора
  /// </summary>
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  public interface IMediator
  {
    /// <summary>
    ///   Асинхронно отправляет команды или события. Данная отправка не требует ответа.
    /// </summary>
    /// <remarks>
    ///   Команда отправляется одному обработчику.
    ///   Событие отправляется множеству обработчиков.
    /// </remarks>
    /// <typeparam name="TMessage">
    ///   Сообщения должны реализовывать следующие интерфейсы:
    ///   <seealso cref="ICommand" />, <seealso cref="IEvent" />.
    /// </typeparam>
    /// <param name="message">Сообщение для отправки.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
    Task SendMessageAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
      where TMessage : class, IMessage;

    /// <summary>
    ///   Асинхронно отправляет запрос единственному обработчику и возвращает результат обработки.
    /// </summary>
    /// <typeparam name="TMessage">Запрос для обработки, должен реализовывать интерфейс <see cref="IRequest" />.</typeparam>
    /// <typeparam name="TResponse">Тип, который обработчик запроса должен вернуть</typeparam>
    /// <param name="message">Ссылка на объект, представляющий запрос.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
    /// <returns>Результат обработки запроса.</returns>
    Task<TResponse> SendMessageAsync<TMessage, TResponse>(TMessage message,
      CancellationToken cancellationToken = default)
      where TMessage : class, IMessage;

    /// <summary>
    ///   Синхронно отправляет команды или события. Данная отправка не требует ответа.
    /// </summary>
    /// <remarks>
    ///   Команда отправляется одному обработчику.
    ///   Событие отправляется множеству обработчиков.
    /// </remarks>
    /// <typeparam name="TMessage">
    ///   Сообщения должны реализовывать следующие интерфейсы:
    ///   <seealso cref="ICommand" />, <seealso cref="IEvent" />.
    /// </typeparam>
    /// <param name="message">Сообщение для отправки.</param>
    void SendMessage<TMessage>(TMessage message) where TMessage : class, IMessage;

    /// <summary>
    ///   Синхронно отправляет запрос единственному обработчику и возвращает результат обработки.
    /// </summary>
    /// <typeparam name="TMessage">Запрос для обработки, должен реализовывать интерфейс <see cref="IRequest" />.</typeparam>
    /// <typeparam name="TResponse">Тип, который обработчик запроса должен вернуть</typeparam>
    /// <param name="message">Ссылка на объект, представляющий запрос.</param>
    /// <returns>Результат обработки запроса.</returns>
    TResponse SendMessage<TMessage, TResponse>(TMessage message)
      where TMessage : class, IMessage;
  }
}