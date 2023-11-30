using Microsoft.Extensions.Configuration;
using ViennaNET.Extensions.Configuration.HashicorpVault.Internals;

namespace ViennaNET.Extensions.Configuration.HashicorpVault;

/// <summary>
///     Представляет базовый класс для <see cref="IConfigurationSource" /> на основе секретов в Hashicorp VaultC.
/// </summary>
public class VaultConfigurationSource : IConfigurationSource
{
    /// <summary>
    ///     Инициализирует новый экземпляр класса <see cref="VaultConfigurationSource" />.
    /// </summary>
    public VaultConfigurationSource()
    {
        ClientOptions = new VaultClientOptions();
        MountPath = "kv/";
        Path = "appsettings.json";
        Version = null;
        ReloadInterval = null;
        OnPollException = null;
        VaultClientBuilder = new VaultClientBuilder();
    }

    /// <summary>
    ///     Ссылка на объект <see cref="HashicorpVault.VaultClientOptions" />.
    /// </summary>
    public VaultClientOptions ClientOptions { get; set; }

    /// <summary>
    ///     Путь к секрету относительный к <see cref="MountPath" />.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    ///     Путь к хранилищу секретов.
    /// </summary>
    public string MountPath { get; set; }

    /// <summary>
    ///     Версия секрета.
    /// </summary>
    public int? Version { get; set; }

    /// <summary>
    ///     Указывает на необходимость, автоматической перезагрузки конфигурации из источника.
    ///     Значение <see langword="true" />, если <see cref="Version" /> = <see langword="null" /> и
    ///     <see cref="ReloadInterval" /> больше <see cref="TimeSpan.Zero" />.
    /// </summary>
    public bool ReloadOnChange =>
        !Version.HasValue && ReloadInterval is not null && ReloadInterval.Value > TimeSpan.Zero;

    /// <summary>
    ///     Интервал времени ожидания между попытками опроса Hashicorp Vault на наличие изменений.
    ///     <c>null</c>, чтобы отключить перезагрузку.
    /// </summary>
    public TimeSpan? ReloadInterval { get; set; }

    /// <summary>
    ///     Ссылка на объект <see cref="IVaultClientBuilder" />.
    /// </summary>
    public IVaultClientBuilder VaultClientBuilder { get; }

    /// <summary>
    ///     Действие, которое следует вызвать, когда <see cref="ReloadOnChange" />
    ///     установлено в <see langword="true" /> и во время опроса возникло какое-либо исключение.
    /// </summary>
    public Action<Exception>? OnPollException { get; set; }

    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new VaultConfigurationProvider(this, new VaultClientBuilder());
    }
}