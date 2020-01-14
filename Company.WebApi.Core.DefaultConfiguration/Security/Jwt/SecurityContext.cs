using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Company.Security;
using Company.Utils;

namespace Company.WebApi.Core.DefaultConfiguration.Security.Jwt
{
  /// <summary>
  /// Контекст, содержащий авторизационные данные пользователя
  /// </summary>
  public class SecurityContext : ISecurityContext
  {
    private readonly IEnumerable<string> _permissions;

    protected SecurityContext() { }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="userName">Имя пользователя</param>
    /// <param name="ip">IP-адрес пользователя</param>
    /// <param name="permissions">Набор полномочий</param>
    public SecurityContext(string userName, string ip, string[] permissions)
    {
      _permissions = permissions.ThrowIfNull(nameof(permissions));

      UserName = GetLogin(userName);
      UserIp = ip;
    }

    /// <summary>
    /// Проверяет наличие полномочий у пользователя
    /// </summary>
    /// <param name="permissions">Набор полномочий</param>
    /// <returns>Есть ли пересечение</returns>
    public Task<bool> HasPermissionsAsync(params string[] permissions)
    {
      var result = _permissions.Intersect(permissions)
                               .Any();

      return Task.FromResult(result);
    }

    /// <summary>
    /// Возвращает все полномочия пользователя
    /// </summary>
    /// <returns>Набор полномочий</returns>
    public Task<IEnumerable<string>> GetUserPermissionsAsync()
    {
      return Task.FromResult(_permissions);
    }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string UserName { get; }

    /// <summary>
    /// IP-адрес пользователя
    /// </summary>
    public string UserIp { get; }

    private static string GetLogin(string userName)
    {
      var index = userName.Return(l => l.IndexOf("\\", StringComparison.Ordinal), -1);
      if (index != -1)
      {
        return userName.Length > index + 1
          ? userName.Substring(index + 1)
          : string.Empty;
      }

      return userName;
    }
  }
}
