using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Представляет тело ответа на запрос <see cref="ScanFileRequest" />.
/// </summary>
/// <param name="Path">
///     Абсолютный путь до просканированного файла.
///     Если экземпляр <see cref="ScanFileResponse" /> является, частью массива <paramref name="SubScanResults" />,
///     тогда  путь до вложенного подобъекта. Обратите внимание, что путь к подобъекту отделен от пути
///     к родительскому объекту двумя косыми чертами (//), например: <c>/home/user/archive.tar//folder/subobject</c>
/// </param>
/// <param name="ScanResult">
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
/// <param name="IsMacrosDetected">Значение true, если в объекте был обнаружен макрос, иначе false.</param>
/// <param name="SubScanResults">
///     Массив, состоящий из результатов сканирования подобъектов, вложенных в просканированный объект. Это поле и все
///     вложенные в него поля будут добавлены в ответ, только если просканированный объект включает вложенные объекты.
/// </param>
public sealed record ScanFileResponse(
    [property: JsonPropertyName("object")] string Path,
    [property: JsonPropertyName("scanResult")] string ScanResult,
    [property: JsonPropertyName("detectionName")] string? DetectionName,
    [property: JsonPropertyName("containsOfficeMacro")] bool IsMacrosDetected,
    [property: JsonPropertyName("subObjectsScanResults")] ScanFileResponse[]? SubScanResults)
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
