using ViennaNET.Messaging.Receiving;
using ViennaNET.Messaging.Sending;

namespace ViennaNET.Messaging.Factories
{
  /// <summary>
  /// Фабрика компонентов для обмена сообщениями
  /// </summary>
  public interface IMessagingComponentFactory
  {
    /// <summary>
    /// Создает отправителя сообщения с сериализацией
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    /// <param name="queueId">Идентификатор очереди</param>
    /// <returns>Отправитель <see cref="ISerializedMessageSender{TMessage}"></see> сообщений</returns>
    ISerializedMessageSender<TMessage> CreateMessageSender<TMessage>(string queueId);

    /// <summary>
    ///   Создает отправителя сообщения
    /// </summary>
    /// <param name="queueId">Идентификатор очереди</param>
    /// <returns>Адаптер очереди <see cref="IMessageAdapter"/></returns>
    IMessageSender CreateMessageSender(string queueId);

    /// <summary>
    /// Создает получатель сообщения
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    /// <param name="queueId">Идентификатор очереди</param>
    /// <returns>Приемник <see cref="IMessageReceiver{TMessage}"></see> сообщений</returns>
    IMessageReceiver<TMessage> CreateMessageReceiver<TMessage>(string queueId);

    /// <summary>
    /// Создает получателя сообщения с поддержкой транзакций
    /// </summary>
    /// <param name="queueId">Идентификатор очереди</param>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    /// <returns>Получатель сообщений</returns>
    ITransactedMessageReceiver<TMessage> CreateTransactedMessageReceiver<TMessage>(string queueId);

    /// <summary>
    /// Создает отправителя сообщения с сериализацией и ожиданием ответа с десериализацией
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <param name="queueId">Идентификатор очереди</param>
    /// <returns>Отправитель <see cref="ISerializedMessageSender{TMessage}"></see> сообщений</returns>
    ISerializedMessageRpcSender<TMessage, TResponse> CreateMessageRpcSender<TMessage, TResponse>(string queueId);
  }
}
