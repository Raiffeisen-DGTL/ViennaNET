namespace ViennaNET.Messaging.Context
{
  /// <summary>
  /// Заголовки для проброски служебной информации между сервисами
  /// </summary>
  public static class MessagingContextHeaders
  {
    /// <summary>
    /// Идентификатор запроса
    /// </summary>
    public const string RequestId = "XRequestId";

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public const string UserId = "XUserId";

    /// <summary>
    /// Домен пользователя
    /// </summary>
    public const string UserDomain = "XUserDomain";

    /// <summary>
    /// IP-адрес создателя первого запроса в цепочке
    /// </summary>
    public const string RequestCallerIp = "XCallerIp";

    /// <summary>
    /// Авторизационные данные создателя первого запроса в цепочке
    /// </summary>
    public const string AuthorizeInfo = "XAuthorize";
  }
}
