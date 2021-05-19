using System.Collections.Generic;

namespace ViennaNET.Messaging.Factories
{
  /// <summary>
  ///   Конструктор адаптера
  /// </summary>
  public interface IMessageAdapterConstructor
  {
    /// <summary>
    ///   Проверяет наличие очереди с заданным именем для данного типа
    /// </summary>
    bool HasQueue(string queueId);

    /// <summary>
    ///   Создание адаптера обмена сообщениями
    /// </summary>
    /// <param name="queueId">Имя очереди</param>
    /// <returns>Адаптер обмена сообщениями</returns>
    IMessageAdapter Create(string queueId);

    /// <summary>
    ///   Создание всех адаптеров обмена сообщениями данного типа
    /// </summary>
    /// <returns>Коллекция адаптеров обмена сообщениями</returns>
    IReadOnlyCollection<IMessageAdapter> CreateAll();
  }
}