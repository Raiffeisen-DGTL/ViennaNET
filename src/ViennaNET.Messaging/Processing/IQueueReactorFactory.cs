namespace ViennaNET.Messaging.Processing
{
  /// <inheritdoc />
  /// <summary>
  ///   Фабрика экземпляров <see cref="T:ViennaNET.Messaging.Processing.IQueueReactor" />
  /// </summary>
  public interface IQueueReactorFactory : IMessageProcessorRegister
  {
    /// <summary>
    /// Создает экземпляр <see cref="IQueueReactor"/>
    /// </summary>
    /// <param name="queueId">Имя очереди</param>
    /// <returns>Реактор очередей <see cref="IQueueReactor"/></returns>
    IQueueReactor CreateQueueReactor(string queueId);
  }
}
