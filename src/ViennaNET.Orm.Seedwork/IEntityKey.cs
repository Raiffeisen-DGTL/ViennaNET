namespace ViennaNET.Orm.Seedwork
{
  /// <summary>
  ///   Интерфейс-маркер сущности доменной модели. Сущности должны реализовывать <see cref="IEntityKey{TKey}" />.
  /// </summary>
  /// <remarks>Используется для ограничений в объявлениях методов (где нельзя использовать generic-интерфейс)</remarks>
  public interface IEntityKey
  {}

  /// <summary>
  ///   Интерфейс-маркер сущности доменной модели
  /// </summary>
  /// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
  public interface IEntityKey<TKey> : IEntityKey
  {
    /// <summary>
    ///   Идентификатор сущности
    /// </summary>
    TKey Id { get; }
  }
}