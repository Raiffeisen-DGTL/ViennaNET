using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Ответа на запрос к API Kaspersky Scan Engine, который содержит обобщенную статистику.
/// </summary>
/// <param name="Data">Ссылка на экземпляр <see cref="Statistics" />.</param>
public sealed record StatisticsResponse([property: JsonPropertyName("statistics")] Statistics Data);

/// <summary>
///     Представляет накопленную службой KSE статистику.
/// </summary>
public sealed record Statistics()
{
    /// <summary>
    ///     Общее количество запросов на сканирование файла или блока памяти и проверки URL-адреса.
    /// </summary>
    [JsonPropertyName("total_requests")]
    public ulong TotalRequests { get; init; }


    /// <summary>
    ///     Количество запросов, на которые Kaspersky Scan Engine вернул результат сканирования
    ///     DETECT, DISINFECTED или DELETED.
    /// </summary>
    [JsonPropertyName("infected_requests")]
    public ulong InfectedRequests { get; init; }

    /// <summary>
    ///     Количество запросов, на которые Kaspersky Scan Engine вернул результат сканирования DISINFECTED или DELETED.
    /// </summary>
    [JsonPropertyName("protected_requests")]
    public ulong ProtectedRequests { get; init; }

    /// <summary>
    ///     Количество запросов, на которые Kaspersky Scan Engine вернул результат сканирования NON_SCANNED (ошибка
    ///     сканирования связана со сканируемым объектом).
    /// </summary>
    [JsonPropertyName("error_requests")]
    public ulong ErrorRequests { get; init; }

    /// <summary>
    ///     Количество запросов, на которые Kaspersky Scan Engine вернул результат сканирования SERVER_ERROR (ошибка
    ///     сканирования не связана со сканируемым объектом).
    /// </summary>
    [JsonPropertyName("engine_errors")]
    public ulong EngineErrors { get; init; }

    /// <summary>
    ///     Размер всех просканированных файлов в байтах.
    /// </summary>
    [JsonPropertyName("processed_data")]
    public ulong ProcessedData { get; init; }

    /// <summary>
    ///     Размер всех просканированных файлов в байтах, отправленных в запросах, на которые Kaspersky Scan Engine
    ///     вернул результат сканирования DETECT, DISINFECTED или DELETED.
    /// </summary>
    [JsonPropertyName("infected_data")]
    public ulong InfectedData { get; init; }

    /// <summary>
    ///     Количество проверенных URL-адресов.
    /// </summary>
    [JsonPropertyName("processed_urls")]
    public ulong ProcessedUrls { get; init; }

    /// <summary>
    ///     Количество URL-адресов, распознанных решением Kaspersky Scan Engine как вредоносные, фишинговые,
    ///     рекламные или легальные программы, которые злоумышленники могут использовать для нанесения ущерба
    ///     компьютеру и данным.
    /// </summary>
    [JsonPropertyName("infected_urls")]
    public ulong InfectedUrls { get; init; }
}
