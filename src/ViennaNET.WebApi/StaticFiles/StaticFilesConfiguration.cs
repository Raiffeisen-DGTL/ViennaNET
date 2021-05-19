namespace ViennaNET.WebApi.StaticFiles
{
  /// <summary>
  /// Объект для чтения конфигурационных данных из секции webApiStaticFiles (работа с файловой системой)
  /// </summary>
  internal class StaticFilesConfiguration
  {
    public string UrlPrefix { get; set; }
    public string Folder { get; set; }
    public int? CacheInterval { get; set; }
  }
}
