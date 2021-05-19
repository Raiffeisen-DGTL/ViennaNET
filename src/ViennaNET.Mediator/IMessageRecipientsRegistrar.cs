using System.Collections.Generic;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator
{
  public interface IMessageRecipientsRegistrar
  {
    /// <summary>
    /// Регистрирует новый экземпляр обработчика событий во время исполнения
    /// </summary>
    /// <typeparam name="TEvent">
    /// Тип события, должен реализовывать: <see cref="IEvent" />.
    /// </typeparam>
    /// <typeparam name="TEventListener">Тип обработчика сообщения: <see cref="IMessageHandler{TMessage}"/>.</typeparam>
    /// <param name="listener">Ссылка на экземпляр обработчика.</param>
    void RegisterEventListener<TEvent, TEventListener>(TEventListener listener)
      where TEvent : class, IEvent
      where TEventListener : IMessageHandler<TEvent>;

    /// <summary>
    /// Регистрирует новый экземпляр обработчика команд во время исполнения
    /// </summary>
    /// <typeparam name="TCommand">
    /// Тип команды, должен реализовывать: <see cref="ICommand" />.
    /// </typeparam>
    /// <typeparam name="TCommandReceiver">Тип обработчика команды: <see cref="IMessageHandler{TMessage}"/>.</typeparam>
    /// <param name="receiver">Ссылка на экземпляр обработчика.</param>
    void RegisterCommandReceiver<TCommand, TCommandReceiver>(TCommandReceiver receiver)
      where TCommand : class, ICommand
      where TCommandReceiver : IMessageHandler<TCommand>;

    /// <summary>
    /// Регистрирует новый экземпляр обработчика запросов во время исполнения
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса <see cref="IRequest"/>.</typeparam>
    /// <typeparam name="TResponse">Тип результата обработки.</typeparam>
    /// <typeparam name="TRequestHandler">Тип обработчика запроса: <see cref = "IMessageHandler{TMessage,TResponse}" />.</typeparam>
    void RegisterRequestHandler<TRequest, TResponse, TRequestHandler>(TRequestHandler registeredHandler)
      where TRequest : class, IRequest
      where TRequestHandler : IMessageHandler<TRequest, TResponse>;

    /// <summary>
    /// Регистрирует переданные экземпляры обработчиков запросов
    /// </summary>
    /// <param name="messageRecipients">Коллекция синхронных обработчиков событий.</param>
    /// <param name="asyncMessageHandlers">Коллекция aсинхронных обработчиков событий</param>
    void Register(IEnumerable<IMessageHandler> messageRecipients, IEnumerable<IMessageHandlerAsync> asyncMessageHandlers);
  }
}
