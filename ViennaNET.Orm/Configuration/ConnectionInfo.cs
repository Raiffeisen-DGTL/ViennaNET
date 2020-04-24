using System;

namespace ViennaNET.Orm.Configuration
{
  /// <summary>
  ///   Содержит данные о подключении к БД
  /// </summary>
  [Serializable]
  public class ConnectionInfo
  {
    /// <summary>
    ///   Тип БД
    /// </summary>
    public string DbServerType { get; set; }

    /// <summary>
    ///   Имя подключения
    /// </summary>
    public string Nick { get; set; }

    /// <summary>
    ///   Строка подключения
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    ///   Признак использования контекста выполнения
    /// </summary>
    public bool UseCallContext { get; set; }

    /// <summary>
    ///   Признак диагностирования сущностей в БД
    /// </summary>
    public bool IsSkipHealthCheckEntity { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
      return
        $"Nick={Nick}, Type={DbServerType}, ConnectionString={ConnectionString}, UseCallContext={UseCallContext}, IsSkipHealthCheckEntity={IsSkipHealthCheckEntity}";
    }
  }
}
