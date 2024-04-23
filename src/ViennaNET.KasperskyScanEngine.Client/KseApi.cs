using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ViennaNET.KasperskyScanEngine.Client.Models;

namespace ViennaNET.KasperskyScanEngine.Client;

internal sealed class KseApi : IKasperskyScanEngineApi
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public KseApi(HttpClient client, IOptionsSnapshot<JsonSerializerOptions> optionsSnapshot)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _jsonOptions = (optionsSnapshot ?? throw new ArgumentNullException(nameof(optionsSnapshot)))
            .Get(IKasperskyScanEngineApi.JsonSerializerOptionsName);
    }

    public async Task<ScanFileResponse?> PostScanFileAsync(ScanFileRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsJsonAsync<ScanFileResponse>("scanfile", request, cancellationToken).ConfigureAwait(false);
    }

    public Task<ScanMemoryResponse?> PostScanMemoryAsync(Stream stream, string? name,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return PostScanMemoryAsyncInternal(stream, name, cancellationToken);
    }

    public async Task<ScanMemoryResponse?> PostScanMemoryAsync(ScanMemoryRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsJsonAsync<ScanMemoryResponse>("scanmemory", request, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<CheckUrlResponse?> PostCheckUrlAsync(CheckUrlRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsJsonAsync<CheckUrlResponse>("checkurl", request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<BasesDateResponse?> GetBaseDateAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<BasesDateResponse>("basesdate", _jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<VersionResponse?> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<VersionResponse>("version", _jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<LicenseInfoResponse?> GetLicenseInfoAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<LicenseInfoResponse>("licenseinfo", _jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<StatisticsResponse?> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<StatisticsResponse>("getstatistics", _jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<ErrorResponse?> PostClearStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var response = (await _client.PostAsync("clearstatistics", null, cancellationToken).ConfigureAwait(false))
            .EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<ErrorResponse>(_jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<UpdateStatusResponse?> PostUpdateStartAsync(CancellationToken cancellationToken = default)
    {
        var response = (await _client
                .PostAsync("update/start", null, cancellationToken)
                .ConfigureAwait(false))
            .EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UpdateStatusResponse>(_jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<UpdateStatusResponse?> GetUpdateStatusAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<UpdateStatusResponse>("update/status", _jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<KsnInfoResponse?> GetKsnInfoAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<KsnInfoResponse>("ksninfo", _jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<ScanMemoryResponse?> PostScanMemoryAsyncInternal(Stream stream, string? name,
        CancellationToken cancellationToken = default)
    {
        using var binaryReader = new BinaryReader(stream);
        var bytes = binaryReader.ReadBytes((int)stream.Length);

        return await PostScanMemoryAsync(new ScanMemoryRequest(bytes, name), cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<TResponse?> PostAsJsonAsync<TResponse>(string uri, object request,
        CancellationToken cancellationToken = default)
    {
        var content = new StringContent(JsonSerializer.Serialize(request, _jsonOptions),
            new MediaTypeHeaderValue(MediaTypeNames.Application.Octet));
        var response = await _client.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var message = $"Непредвиденная ошибка при выполнении запроса: {uri}";

            if (response.Content.Headers.ContentLength > 0)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_jsonOptions, cancellationToken)
                    .ConfigureAwait(false);

                if (error is not null)
                {
                    message = error.Message;
                }
            }

            throw new InvalidOperationException(message);
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, 
            cancellationToken).ConfigureAwait(false);
    }
}