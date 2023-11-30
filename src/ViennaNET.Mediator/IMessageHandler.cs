using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator
{
  /// <summary>
  ///   Интерфейс-маркер обработчика сообщений
  /// </summary>
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  public interface IMessageHandler
  {}

  /// <summary>
  ///   Типизированный интерфейс-маркер обработчика сообщений
  /// </summary>
  /// <typeparam name="TMessage">Type of the message.</typeparam>
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  public interface IMessageHandler<in TMessage> : IMessageHandler
    where TMessage : class, IMessage
  {
    /// <summary>
    ///   Синхронно обрабатывает сообщение, не возвращая результатов
    /// </summary>
    /// <param name="message">Ссылка на сообщение.</param>
    void Handle(TMessage message);
  }

  /// <summary>
  ///   Типизированный интерфейс-маркер обработчика запросов, предполагающего ответ
  /// </summary>
  /// <typeparam name="TMessage">Тип сообщения.</typeparam>
  /// <typeparam name="TResponse">Тип результата.</typeparam>
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  public interface IMessageHandler<in TMessage, TResponse> : IMessageHandler
    where TMessage : class, IMessage
  {
    /// <summary>
    ///   Синхронно обрабатывает сообщение, возвращая результат.
    /// </summary>
    /// <param name="message">Ссылка на сообщение.</param>
    /// <returns>Результат обработки.</returns>
    TResponse Handle(TMessage message);
  }
}