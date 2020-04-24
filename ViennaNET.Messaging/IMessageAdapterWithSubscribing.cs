using System;
using System.Threading.Tasks;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging
{
  /// <summary>
  ///   Интерфейс адаптера для работы с очередью с использованием шаблона "Наблюдатель"
  /// </summary>
  public interface IMessageAdapterWithSubscribing : IMessageAdapter
  {
    /// <summary>
    ///   Подписывает функцию обратного вызова на получение сообщений из очереди
    /// </summary>
    /// <param name="handler">Функция обратного вызова</param>
    void Subscribe(Func<BaseMessage, Task> handler);

    /// <summary>
    ///   Отписывает функцию обратного вызова от получение сообщений из очереди
    /// </summary>
    void Unsubscribe();

    /// <summary>
    ///   Реализует возможность RPC-запроса к очереди
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>Сообщение</returns>
    Task<BaseMessage> RequestAndWaitResponse(BaseMessage message);

    /// <summary>
    ///   Отвечает на сообщение из очереди
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>Сообщение</returns>
    BaseMessage Reply(BaseMessage message);
  }
}