using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Определяет общие параметры запросов
///     <a href="https://support.kaspersky.com/help/ScanEngine/2.1/ru-RU/193000.htm">
///         к API Kaspersky Scan Engine.
///     </a>
/// </summary>
[JsonDerivedType(typeof(ScanFileRequest))]
[JsonDerivedType(typeof(ScanMemoryRequest))]
[JsonDerivedType(typeof(CheckUrlRequest))]
[SuppressMessage("ReSharper", "EmptyConstructor")]
#pragma warning disable S1694
public abstract record ScanRequestBase() // Для предотвращения создания конструктора по умолчанию компилятором
#pragma warning restore S1694
{
    /// <summary>
    ///     Необязательный параметр, определяющий время ожидания окончания проверки объекта в миллисекундах (мс).
    ///     <para>
    ///         Если не определён, будет использован параметр конфигурации
    ///         <see cref="KseClientOption" />.
    ///     </para>
    /// </summary>
    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }

    /// <summary>
    ///     Необязательный параметр, определяющий, должен ли возвращаемый в ответе массив subObjectsScanResults,
    ///     содержать объекты с результатом сканирования: CLEAN.
    ///     <para>
    ///         Если значение <see langword="true" />, такие объекты опускаются, иначе они будут добавлены в ответ.
    ///         Если значение не указано в запросе, сервер использует значение по умолчанию: <see langword="true" />.
    ///     </para>
    /// </summary>
    [JsonPropertyName("omitCleanSubobjectResults")]
    public bool? OmitCleanSubobjectResults { get; init; }

    /// <summary>
    ///     Необязательный URL-адрес, который будет использован как контекст для запроса на сканирование.
    ///     <para>
    ///         Это поле можно использовать для
    ///         <a href="https://support.kaspersky.com/help/ScanEngine/2.1/ru-RU/181065.htm">
    ///             повышения коэффициента обнаружения.
    ///         </a>
    ///     </para>
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    ///     Необязательный IP-адрес узла, на котором размещен просканированный URL-адрес.
    ///     <para>
    ///         Это поле можно использовать для
    ///         <a href="https://support.kaspersky.com/help/ScanEngine/2.1/ru-RU/181065.htm">
    ///             повышения коэффициента обнаружения.
    ///         </a>
    ///     </para>
    /// </summary>
    [JsonPropertyName("hostIp")]
    public string? HostIp { get; init; }

    /// <summary>
    ///     Необязательный параметр, определяющий заголовки HTTP-запроса, извлеченные из HTTP-трафика при загрузке
    ///     сканируемого файла с внешних ресурсов.
    ///     <para>
    ///         Это поле можно использовать для
    ///         <a href="https://support.kaspersky.com/help/ScanEngine/2.1/ru-RU/181065.htm">
    ///             повышения коэффициента обнаружения.
    ///         </a>
    ///     </para>
    /// </summary>
    [JsonPropertyName("requestHeaders")]
    public string? RequestHeaders { get; init; }

    /// <summary>
    ///     Необязательный параметр, определяющий заголовки HTTP-ответа, извлеченные из HTTP-трафика при загрузке
    ///     сканируемого файла с внешних ресурсов.
    ///     <para>
    ///         Это поле можно использовать для
    ///         <a href="https://support.kaspersky.com/help/ScanEngine/2.1/ru-RU/181065.htm">
    ///             повышения коэффициента обнаружения.
    ///         </a>
    ///     </para>
    /// </summary>
    [JsonPropertyName("responseHeaders")]
    public string? ResponseHeaders { get; init; }
}