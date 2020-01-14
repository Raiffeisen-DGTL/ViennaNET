using System.Collections.Generic;

namespace Company.Security.Jwt
{
  public interface IJwtTokenFactory
  {
    /// <summary>
    /// Создает новый токен
    /// </summary>
    /// <param name="userName">Имя пользователя</param>
    /// <param name="permissions">Список доступов</param>
    /// <returns>JWT-токен</returns>
    string Create(string userName, string[] permissions);

    /// <summary>
    ///   Создает новый токен
    /// </summary>
    /// <param name="userName">Имя пользователя</param>
    /// <param name="permissions">Список доступов</param>
    /// <param name="additionalData">Словарь с дополнительными атрибутами, которые нужно добавить в токен</param>
    /// <returns>JWT-токен</returns>
    string Create(string userName, string[] permissions, IDictionary<string, object> additionalData);
  }
}
