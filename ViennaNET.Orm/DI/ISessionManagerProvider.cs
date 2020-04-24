namespace ViennaNET.Orm.DI
{
  /// <summary>
  /// Провайдер, возвращающий менеджер сессий. Позволяет отделить механизм
  /// получения менеджера сессий от работы репозиториев
  /// </summary>
  public interface ISessionManagerProvider
  {
    /// <summary>
    /// Возвращает менеджер сессий
    /// </summary>
    /// <returns>Менеджер сессий</returns>
    ISessionManager GetSessionManager();
  }
}
