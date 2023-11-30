using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using ViennaNET.Extensions.Configuration.HashicorpVault.Internals;

namespace ViennaNET.Extensions.Configuration.HashicorpVault;

/// <summary>
///     Представляет базовый класс для <see cref="ConfigurationProvider" /> на основе ресурсов Kubernetes.
/// </summary>
public sealed class VaultConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private bool _disposed;
    private int _loadedVersion;
    private Task? _pollingTask;

    /// <summary>
    ///     Инициализирует новый экземпляр с указанным источником, и настроенным клиентом API K8S.
    /// </summary>
    /// <param name="source">Ссылка на <see cref="VaultConfigurationSource" />.</param>
    /// <param name="builder">Ссылка на <see cref="IVaultClientBuilder" />.</param>
    /// <exception cref="ArgumentNullException">
    ///     Возникает, если <paramref name="source" /> = <see langword="null" />.
    /// </exception>
    internal VaultConfigurationProvider(VaultConfigurationSource source, IVaultClientBuilder builder)
    {

        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        Source = source ?? throw new ArgumentNullException(nameof(source));
        Client = builder.Build(Source.ClientOptions);

        _cancellationTokenSource = new CancellationTokenSource();
        _pollingTask = null;
    }

    /// <summary>
    ///     Ссылка на <see cref="VaultConfigurationSource" />.
    /// </summary>
    public VaultConfigurationSource Source { get; }

    /// <summary>
    ///     Ссылка на <see cref="IVaultClient" />.
    /// </summary>
    public IVaultClient Client { get; }

    /// <inheritdoc />
    public override void Load()
    {
        LoadAsync().GetAwaiter().GetResult();
    }

    private async Task LoadAsync()
    {
        var secret = await Client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            Source.Path, Source.Version, Source.MountPath);

        // Когда перезагрузка включена, конфигурация не обязательна по умолчанию.
        if (Source.ReloadOnChange && secret.Data.Metadata.Destroyed)
        {
            Data = new Dictionary<string, string>();
        }
        
        if (_loadedVersion != secret.Data.Metadata.Version)
        {
            Data = KeyValueParser.Parse(secret.Data.Data);

            _loadedVersion = secret.Data.Metadata.Version;

            OnReload();
        }

        if (_pollingTask is null && Source.ReloadOnChange)
        {
            _pollingTask = PollAsync();
        }
    }

    private async Task PollAsync()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            await Task.Delay(Source.ReloadInterval!.Value, _cancellationTokenSource.Token).ConfigureAwait(false);

            try
            {
                await LoadAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (Source.OnPollException is { } handler)
                {
                    handler(e);
                }
            }
        }
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage(Justification = "Завершение делегируются. Нет логики для тестирования")]
    public void Dispose()
    {
        Dispose(true);
        // SuppressFinalize не нужен, так как класс запечатан, а свой деструктор у него не объявлен.
    }

    /// <summary>
    ///     Вызывает <see cref="Client" />.<see cref="IDisposable.Dispose" />.
    /// </summary>
    /// <param name="disposing">
    ///     Указывает, необходимо ли освобождать ресурсы. Предотвращает повторное освобождение.
    /// </param>
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!_disposed)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            _disposed = true;
        }
    }
}