namespace ViennaNET.Orm.Seedwork
{
  /// <summary>
  /// Позволяет создавать репозиторий для работы с сущностями
  /// </summary>
  public interface IEntityRepositoryFactory
  {
    /// <summary>
    /// Создает репозиторий для работы с сущностями
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    /// <returns>Репозиторий</returns>
    IEntityRepository<T> Create<T>() where T : class;
  }
}
