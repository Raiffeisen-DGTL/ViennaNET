namespace Company.WebApi.Core.DefaultConfiguration.HttpClients
{
  /// <summary>
  /// Заголовки для проброски служебной информации между сервисами
  /// </summary>
  public static class CompanyHeaders
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
    public const string RequestHeaderCallerIp = "X-Caller-Ip";
  }
}
