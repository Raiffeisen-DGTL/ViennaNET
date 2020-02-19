using System.Collections.Generic;
using System.Threading.Tasks;

namespace ViennaNET.Security
{
  /// <summary>
  /// Контекст, содержащий авторизационные данные пользователя
  /// </summary>
  public interface ISecurityContext
  {
    /// <summary>
    /// Имя пользователя
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// IP-адрес пользователя
    /// </summary>
    string UserIp { get; }

    /// <summary>
    /// Проверяет наличие полномочий у пользователя
    /// </summary>
    /// <param name="permissions">Набор полномочий</param>
    /// <returns>Есть ли пересечение</returns>
    Task<bool> HasPermissionsAsync(params string[] permissions);

    /// <summary>
    /// Возвращает все полномочия пользователя
    /// </summary>
    /// <returns>Набор полномочий</returns>
    Task<IEnumerable<string>> GetUserPermissionsAsync();
  }
}
