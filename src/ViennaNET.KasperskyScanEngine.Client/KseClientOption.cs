using ViennaNET.Extensions.Http;

namespace ViennaNET.KasperskyScanEngine.Client;

/// <summary>
///     Параметры конфигурации HTTP клиента службы
///     <a href="https://support.kaspersky.com/ScanEngine/1.0/en-US/179679.htm">
///         Kaspersky Scan Engine.
///     </a>
/// </summary>
public sealed class KseClientOption : ClientOptionsBase
{
    /// <summary>
    ///     Имя секции, в поставщиках конфигурации используемых клиентом.
    /// </summary>
    public const string SectionName = DefaultSectionRootName + "KasperskyScanEngine";

    /// <summary>
    ///     Если определён, тогда будет отправлен в заголовке Authorization, во всех запросах к KSE.
    /// </summary>
    public string? AuthorizationToken { get; set; }
}