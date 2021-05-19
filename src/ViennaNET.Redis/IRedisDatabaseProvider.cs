using System.Collections.Generic;
using System.Net;

namespace ViennaNET.Redis
{
  /// <summary>
  /// Провайдер для работы с БД Redis
  /// </summary>
  public interface IRedisDatabaseProvider
  {
    /// <summary>
    /// Получает абстрактную БД для выполнения операций с Redis
    /// </summary>
    /// <param name="useCompression">Признак использования сжатия</param>
    /// <param name="database">Номер базы данных</param>
    /// <param name="asyncState">Состояние асинхронного вызова</param>
    IRedisDatabase GetDatabase(bool useCompression = false, int database = 0, object asyncState = null);

    /// <summary>
    /// Получает абстрактный сервер Redis
    /// </summary>
    /// <param name="endPoint">Имя сервера</param>
    /// <param name="asyncState">Состояние асинхронного вызова</param>
    IRedisServer GetServer(EndPoint endPoint, object asyncState = null);

    /// <summary>
    /// Получает все абстрактные сервера Redis
    /// </summary>
    /// <param name="asyncState">Состояние асинхронного вызова</param>
    IEnumerable<IRedisServer> GetAllServers(object asyncState = null);
  }
}
