using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Определяет параметры запроса к API Kaspersky Scan Engine,
///     на сканирование локального файла: /api/{ver}/scanfile.
///     <b>У Scan Engine должеен быть доступ к файлу по указанному пути.</b>
/// </summary>
/// <param name="Path">Абсолютный путь до файла, который нужно просканировать.</param>
public sealed record ScanFileRequest([property: JsonPropertyName("object")] string Path) : ScanRequestBase();

