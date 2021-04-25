using System;

namespace ViennaNET.Orm.Factories
{
  /// <summary>
  /// Провайдер для регистрации сущностей и получения имен подключения сущностей
  /// </summary>
  public interface IApplicationContextProvider
  {
    /// <summary>
    /// Получает имя подключения именованного запроса
    /// </summary>
    /// <param name="namedQuery">Имя запроса</param>
    /// <returns>Имя подключения</returns>
    string GetNickForNamedQuery(string namedQuery);

    /// <summary>
    /// Получает имя подключения сущности
    /// </summary>
    /// <param name="type">Тип сущности</param>
    /// <returns>Имя подключения</returns>
    string GetNick(Type type);

    /// <summary>
    /// Получает имя подключения команды
    /// </summary>
    /// <param name="type">Тип сущности</param>
    /// <returns>Имя подключения</returns>
    string GetNickForCommand(Type type);
  }
}
