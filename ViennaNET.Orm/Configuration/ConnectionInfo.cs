using System;
using ViennaNET.Protection;

namespace ViennaNET.Orm.Configuration
{
  /// <summary>
  ///   Содержит данные о подключении к БД
  /// </summary>
  [Serializable]
  public class ConnectionInfo
  {
    private string _password;

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
    ///   Зашифрованный пароль
    /// </summary>
    public string EncPassword
    {
      get => _password;
      set =>
        _password = !string.IsNullOrEmpty(value)
          ? value.Unprotect()
          : string.Empty;
    }

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
