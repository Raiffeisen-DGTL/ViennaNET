using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Ответа на запрос к API Kaspersky Scan Engine, который содержит лицензионную информацию: api/{ver}/licenseinfo.
/// </summary>
/// <param name="ExpirationDate">Дата, до которой действителен используемый файл ключа или билет.</param>
/// <param name="KeyFileName">Имя используемого файла ключа. (для офлайн режима)</param>
/// <param name="ActivationCode">Используемый код активации (для онлайн режима).</param>
/// <param name="TicketExpired">
///     Сообщение, которое будет включено в ответ, если истек срок действия лицензионного билета.
/// </param>
public sealed record LicenseInfoResponse(
    [property: JsonPropertyName("licenseExpirationDate")] string ExpirationDate,
    [property: JsonPropertyName("licenseName")] string? KeyFileName = null,
    [property: JsonPropertyName("activationCode")] string? ActivationCode = null,
    [property: JsonPropertyName("ticketExpired")] string? TicketExpired = null)
{
    /// <summary>
    ///     Получает значение, определяющее режим активации лицензии, если значение <see langword="true" />,
    ///     тогда используется Offline режим, иначе Online.
    /// </summary>
    [JsonIgnore]
    public bool IsOfflineActivationMode => KeyFileName is not null && ActivationCode is null;
}
