namespace ViennaNET.Messaging.Factories
{
  /// <summary>
  ///   Адаптер обмена сообщениями
  /// </summary>
  public interface IMessageAdapterFactory
  {
    /// <summary>
    /// Создает адаптера обмена сообщениями по имени очереди
    /// </summary>
    /// <param name="queueId">Имя очереди</param>
    /// <param name="isDiagnostic">Признак диагоностики</param>
    /// <returns>Адаптер очереди <see cref="IMessageAdapter"/></returns>
    IMessageAdapter Create(string queueId, bool isDiagnostic);
  }
}
