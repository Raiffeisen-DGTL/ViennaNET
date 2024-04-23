using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Определяет параметры запроса к API Kaspersky Scan Engine,
///     на сканирование объекта в памяти: /api/{ver}/scanmemory.
/// </summary>
/// <param name="Content">Содержимое сканируемого файла в виде строки в кодировке Base64.</param>
/// <param name="Name">Имя сканируемого файла.</param>
public sealed record ScanMemoryRequest(
        [property: JsonPropertyName("object")] byte[] Content,
        [property: JsonPropertyName("name")] string? Name = null)
    : ScanRequestBase;
