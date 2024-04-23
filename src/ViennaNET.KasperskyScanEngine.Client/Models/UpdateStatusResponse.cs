using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Ответа на запрос к API Kaspersky Scan Engine статуса обновления БД угроз: api/{ver}/update/start|status.
/// </summary>
/// <param name="Status">Статус обновления: не выполняется, в процессе и т. д.</param>
/// <param name="Progress">
///     Прогресс обновления антивирусных баз в процентах.
///     Включается в ответ, только если <paramref name="Status"/> = "in progress".
/// </param>
/// <param name="LastUpdateResult">
///     Результат последнего обновления. Включается в ответ, только если <paramref name="Status"/> = "not started".
///     Статус может иметь следующие значения:
///     <list type="bullet">
///         <item><term>success</term></item>
///         <item><term>all components are up to date</term></item>
///         <item><term>invalid update sources</term></item>
///         <item><term>not all components are updated</term></item>
///         <item><term>download error</term></item>
///         <item><term>error while updating</term></item>
///         <item>
///             <term>eror {ErrorCode} occurred</term>
///             <description>
///                 где ErrorCode – это код ошибки, полученный от Kaspersky Scan Engine.
///             </description>
///          </item>
///     </list> 
/// </param>
/// <param name="LastUpdateTime">
///     Дата и время последнего обновления. 
///     Включается в ответ, только если <paramref name="Status"/> = "not started".
/// </param>
/// <param name="ActionNeeded">
///     <b>Только для протокола версии >= 3.1.</b>
///     Действие, которое необходимо выполнить, прежде чем применить обновления. 
///     Статус может иметь следующие значения:
///     <list type="bullet">
///         <item>
///              <term>Product restart needed</term>
///              <description>Это означает, что необходимо перезапустить Kaspersky Scan Engine.</description>
///         </item>
///         <item>
///              <term>OS reboot needed</term>
///              <description>
///                 Это означает, что необходимо перезагрузить операционную систему, 
///                 в которой работает Kaspersky Scan Engine.
///              </description>
///         </item>
///     </list> 
/// </param>
/// <param name="ActionApplyPeriod">
///     <b>Только для протокола версии >= 3.1.</b>
///     Рекомендуемый период (в часах) для выполнения действия, указанного в <paramref name="ActionNeeded" />.
/// </param>
public sealed record UpdateStatusResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("progress")] string? Progress,
    [property: JsonPropertyName("last_update_result")] string? LastUpdateResult,
    [property: JsonPropertyName("last_update_time")] string? LastUpdateTime,
    [property: JsonPropertyName("action_needed")] string? ActionNeeded,
    [property: JsonPropertyName("action_apply_period")] int? ActionApplyPeriod);
