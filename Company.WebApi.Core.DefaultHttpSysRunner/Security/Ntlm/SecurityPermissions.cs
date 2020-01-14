namespace Company.WebApi.Core.DefaultHttpSysRunner.Security.Ntlm
{
  /// <summary>
  /// Объект для получения полномочий пользователя из ответа Security-сервиса
  /// </summary>
  internal class SecurityPermissions
  {
    /// <summary>
    /// Полномочия пользователя
    /// </summary>
    public string[] Permissions { get; set; }

    /// <summary>
    /// Login пользователя
    /// </summary>
    public string Login { get; set; }
  }
}
