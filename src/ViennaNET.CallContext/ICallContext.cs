namespace ViennaNET.CallContext
{
  public interface ICallContext
  {
    /// <summary>
    /// Идентификатор запроса
    /// </summary>
    string RequestId { get; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// Домен пользователя
    /// </summary>
    string UserDomain { get; }

    /// <summary>
    /// IP-адрес создателя первого запроса в цепочке
    /// </summary>
    string RequestCallerIp { get; }

    /// <summary>
    /// Авторизационный токен пользователя
    /// </summary>
    string AuthorizeInfo { get; }
  }
}
