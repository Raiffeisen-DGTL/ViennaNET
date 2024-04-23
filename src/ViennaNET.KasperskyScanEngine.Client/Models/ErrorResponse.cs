using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Представляет тело ответа, когда запрос сканирования был обработан с ошибкой.
/// </summary>
/// <param name="Message">Сообщение об ошибке.</param>
public sealed record ErrorResponse([property: JsonPropertyName("error")] string Message);
