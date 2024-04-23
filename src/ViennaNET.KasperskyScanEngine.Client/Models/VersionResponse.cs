using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Ответа на запрос к API Kaspersky Scan Engine версии KAV SDK: api/{ver}/version.
/// </summary>
/// <param name="KavSdkVersion">
///     Содержит текущую версию KAV SDK в формате:
///     MajorVersion.MinorVersion.BuildNumber.Revision.
/// </param>
public sealed record VersionResponse([property: JsonPropertyName("KAVSDKVersion")] Version KavSdkVersion);