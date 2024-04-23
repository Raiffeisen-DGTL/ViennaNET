using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Ответа на запрос к API Kaspersky Scan Engine статуса обновления БД угроз: api/{ver}/ksninfo.
/// </summary>
/// <param name="Status">Статус KSN.</param>
/// <param name="WhiteApplications">Белый список приложений.</param>
/// <param name="MalwareApplications">Список вредоносных приложений.</param>
/// <param name="BlockedThreats">Заблокированные угрозы.</param>
/// <param name="Region">KSN регион.</param>
/// <param name="ResponseTimestamp">Метка времени для ответа на запрос.</param>
public sealed record KsnInfoResponse(
    [property: JsonPropertyName("ksnStatus")] string Status,
    [property: JsonPropertyName("whiteApplications")] long? WhiteApplications,
    [property: JsonPropertyName("malwareApplications")] long? MalwareApplications,
    [property: JsonPropertyName("blockedThreats")] long? BlockedThreats,
    [property: JsonPropertyName("region")] string? Region,
    [property: JsonPropertyName("responseTimestamp")] string? ResponseTimestamp);
