using System.Collections.Generic;

namespace ViennaNET.Diagnostic
{
  /// <summary>
  /// Результат диагностики сервиса
  /// </summary>
  public class DiagnoseResult
  {
    public string Name { get; set; }
    public string Host { get; set; }
    public string Version { get; set; }
    public bool HasErrors { get; set; }
    public IEnumerable<EndpointResult> Results { get; set; }
  }

  /// <summary>
  /// Результат диагностики подключения к сторонним сущностям
  /// </summary>
  public class EndpointResult
  {
    public string Name { get; set; }
    public string Url { get; set; }
    public string Status { get; set; }
    public string Error { get; set; }
  }
}
