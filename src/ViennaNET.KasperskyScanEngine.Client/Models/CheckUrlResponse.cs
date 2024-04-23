using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Представляет тело ответа на запрос <see cref="CheckUrlResponse" />.
/// </summary>
/// <param name="Url">Проверенный URL-адрес.</param>
/// /// <param name = "ScanResult" >
///     Результат сканирования. 
///     Может иметь следующие значения:
///     <list type="bullet">
///         <item>
///             <term>CLEAN</term>
///         </item>
///         <item>
///             <term>DETECT</term>
///         </item>
///         <item>
///             <term>DISINFECTED</term>
///         </item>
///         <item>
///             <term>DELETED</term>
///         </item>
///         <item>
///             <term>NON_SCANNED</term>
///         </item>
///         <item>
///             <term>SERVER_ERROR</term>
///         </item>
///     </list>
///     <para>
///         Для запросов <see cref="ScanFileRequest" /> и <see cref="ScanMemoryRequest" />, к результату NON_SCANNED
///         может быть добавлена причина. Возможные варианты значений:
///         <list type="bullet">
///             <item>
///                 <term>NON_SCANNED (CANCELED)</term>
///                 <description>Сканирование отменено.</description>
///             </item>
///             <item>
///                 <term>NON_SCANNED (CORRUPTED)</term>
///                 <description>The library or executable built from a compilation.</description>
///             </item>
///             <item>
///                 <term>NON_SCANNED (ACCESS DENIED)</term>
///                 <description>Доступ к объекту запрещен.</description>
///             </item>
///             <item>
///                 <term>NON_SCANNED (SKIPPED)</term>
///                 <description>Сканирование пропущено.</description>
///             </item>
///             <item>
///                 <term>NON_SCANNED (PASSWORD PROTECTED)</term>
///                 <description>Проверяемый объект защищен паролем.</description>
///             </item>
///             <item>
///                 <term>NON_SCANNED</term>
///                 <description>Причина, по которой объект не сканируется, не определена.</description>
///             </item>
///         </list>
///     </para>
/// </param>
/// <param name="DetectionName">
///     Имя обнаруженного вредоносного объекта в классификации "Лаборатории Касперского".
///     В ответе на запрос <see cref="CheckUrlRequest" /> возможные значения определены ниже:
///     <list type="bullet">
///         <item>
///             <term>PHISHING_URL</term>
///         </item>
///         <item>
///             <term>MALICIOUS_URL</term>
///         </item>
///         <item>
///             <term>ADWARE_URL</term>
///         </item>
///         <item>
///             <term>RISKWARE_URL</term>
///         </item>
///     </list>
/// </param>
public sealed record CheckUrlResponse(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("scanResult")] string ScanResult,
    [property: JsonPropertyName("detectionName")] string? DetectionName)
{
    /// <summary>
    ///     Возвращает значение, определяющее, что KSE не обнаружил угроз.
    /// </summary>
    [JsonIgnore]
    public bool IsClean => ScanResult == "CLEAN";

    /// <summary>
    ///     Возвращает значение, определяющее обноружил ли KSE угрозу.
    /// </summary>
    [JsonIgnore]
    public bool IsDetect => ScanResult == "DETECT";
}