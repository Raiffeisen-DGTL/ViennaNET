using System.Text.Json;
using System.Text.Json.Serialization;
using ViennaNET.KasperskyScanEngine.Client.Models;

namespace ViennaNET.KasperskyScanEngine.Client;

/// <summary>
///     Представляет контекст сериализации JSON для оптимизации производительности сериализации и десереализации
///     типов используемых API <see cref="IKasperskyScanEngineApi" />.
/// </summary>
[JsonSerializable(typeof(ScanRequestBase))]
[JsonSerializable(typeof(ScanFileRequest))]
[JsonSerializable(typeof(ScanMemoryRequest))]
[JsonSerializable(typeof(CheckUrlRequest))]
[JsonSerializable(typeof(StatisticsResponse))]
[JsonSerializable(typeof(Statistics))]
[JsonSerializable(typeof(UpdateStatusResponse))]
[JsonSerializable(typeof(KsnInfoResponse))]
[JsonSerializable(typeof(LicenseInfoResponse))]
[JsonSerializable(typeof(BasesDateResponse))]
[JsonSerializable(typeof(VersionResponse))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(ScanFileResponse))]
[JsonSerializable(typeof(ScanMemoryResponse))]
[JsonSerializable(typeof(CheckUrlResponse))]
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web,
    AllowTrailingCommas = true,
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class KseClientSerializerContext : JsonSerializerContext
{
}