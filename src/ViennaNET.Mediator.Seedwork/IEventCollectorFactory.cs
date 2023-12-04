namespace ViennaNET.Mediator.Seedwork
{
  /// <summary>
  ///   Фабрика для создания <see cref="IEventCollector" />
  /// </summary>
  public interface IEventCollectorFactory
  {
    /// <summary>
    ///   Создает новый коллектор
    /// </summary>
    /// <returns>Ссылка на объект <see cref="IEventCollector" />.</returns>
    IEventCollector Create();
  }
}