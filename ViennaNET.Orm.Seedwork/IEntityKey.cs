namespace ViennaNET.Orm.Seedwork
{
  /// <summary>
  /// Интерфейс-маркер сущности доменной модели
  /// </summary>
  /// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
  public interface IEntityKey<TKey>
  {
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    TKey Id { get; }
  }
}
