using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Определяет параметры запроса к API Kaspersky Scan Engine,
///     на сканирование указанного URL: /api/{ver}/checkurl.
/// </summary>
/// <param name="Url">
///     Обязательный параметр: URL-адрес, который будет использован как контекст для запроса на сканирование.
/// </param>
public sealed record CheckUrlRequest(string Url) : ScanRequestBase()
{
    /// <summary>
    ///     Инициализирует новый экземпляр с заданными значениями полей.
    /// </summary>
    [JsonPropertyName("url")]
    public new string Url { get; set; } = Url;
}