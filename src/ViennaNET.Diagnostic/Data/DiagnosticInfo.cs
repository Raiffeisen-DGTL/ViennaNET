namespace ViennaNET.Diagnostic.Data
{
  /// <summary>
  ///   Содержит результат диагностики функции сервиса
  /// </summary>
  public class DiagnosticInfo
  {
    /// <summary>
    ///   Инициализирует экземпляр передаваемыми параметрами
    /// </summary>
    /// <param name="name">Имя диагностируемой функции</param>
    /// <param name="url">URL диагностируемой функции, если применимо</param>
    /// <param name="status">Статус диагностики</param>
    /// <param name="version">Версия диагностируемой функции. Необязательное поле</param>
    /// <param name="error">Ошибка диагностики</param>
    /// <param name="isSkipResult">Пропустить результат диагностики</param>
    public DiagnosticInfo(
      string name, string url, DiagnosticStatus status = DiagnosticStatus.Ok, string version = "", string error = "", bool isSkipResult = false)
    {
      Name = name;
      Url = url;
      Version = version;
      Status = status;
      Error = error;
      IsSkipResult = isSkipResult;
    }

    /// <summary>
    ///   Ошибка диагностики
    /// </summary>
    public string Error { get; }

    /// <summary>
    ///   Статус диагностики
    /// </summary>
    public DiagnosticStatus Status { get; }

    /// <summary>
    ///   Имя диагностируемой функции
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///   URL диагностируемой функции
    /// </summary>
    public string Url { get; }

    /// <summary>
    ///   <param name="version">Версия диагностируемой функции</param>
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Пропустить результат диагностики 
    /// </summary>
    public bool IsSkipResult { get; }
  }
}
