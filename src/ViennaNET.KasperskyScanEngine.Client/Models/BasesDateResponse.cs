using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client.Models;

/// <summary>
///     Ответа на запрос к API Kaspersky Scan Engine, который содержит версию антивирусных баз: api/{ver}/basesdate.
/// </summary>
/// <param name="DatabaseVersion">Текущая версия антивирусных баз в формате: DD.MM.YYYY hh:mm GMT.</param>
public sealed record BasesDateResponse([property: JsonPropertyName("databaseVersion")] string DatabaseVersion);
