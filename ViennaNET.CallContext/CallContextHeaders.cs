namespace ViennaNET.CallContext
{
  /// <summary>
  /// Заголовки для проброски служебной информации между сервисами
  /// </summary>
  public static class CallContextHeaders
  {
    /// <summary>
    /// Идентификатор запроса
    /// </summary>
    public const string RequestId = "X-Request-Id";

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public const string UserId = "X-User-Id";

    /// <summary>
    /// Домен пользователя
    /// </summary>
    public const string UserDomain = "X-User-Domain";

    /// <summary>
    /// IP-адрес создателя первого запроса в цепочке
    /// </summary>
    public const string RequestCallerIp = "X-Caller-Ip";

    /// <summary>
    /// Авторизационные данные создателя первого запроса в цепочке
    /// </summary>
    public const string AuthorizeInfo = "X-Authorize";
  }
}
