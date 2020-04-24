namespace ViennaNET.Orm.DI
{
  /// <summary>
  /// Провайдер контекста вызова
  /// </summary>
  public interface ICallContextProvider
  {
    /// <summary>
    /// Возвращает имя пользователя, выполняющего вызов
    /// </summary>
    /// <returns>Имя пользователя</returns>
    string GetUserName();

    /// <summary>
    /// Возвращает IP пользователя, выполняющего вызов
    /// </summary>
    /// <returns>IP пользователя</returns>
    string GetUserIp();
  }
}
