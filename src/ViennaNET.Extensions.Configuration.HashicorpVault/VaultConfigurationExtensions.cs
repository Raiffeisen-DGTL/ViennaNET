using Microsoft.Extensions.Configuration;

namespace ViennaNET.Extensions.Configuration.HashicorpVault;

/// <summary>
///     Предоставляет методы расширения для <see cref="IConfigurationBuilder" />.
/// </summary>
public static class VaultConfigurationExtensions
{
    /// <summary>
    ///     Добавляет источник конфигурации на основе секретов HashicorpVault, с заданными параметрами.
    /// </summary>
    /// <remarks>
    ///     Добавленный поставщик, единожды загружает указанный секреты, и не отслеживает появление новых версий.
    /// </remarks>
    /// <param name="builder">Ссылка на объект <see cref="IConfigurationBuilder" />.</param>
    /// <param name="configureClient">Действие, которое настраивает <see cref="VaultClientOptions" />.</param>
    /// <param name="path">Путь к секрету, относительный к <paramref name="mountPath" />.</param>
    /// <param name="version">Версия, которую следует загрузить.</param>
    /// <param name="mountPath">Путь к ранилищу секретов. Значение по умолчанию "kv/".</param>
    /// <returns>Ссылка на объект <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddVault(
        this IConfigurationBuilder builder,
        Action<VaultClientOptions> configureClient,
        string path,
        int version,
        string mountPath = "kv/")
    {
        var clientOptions = new VaultClientOptions();

        configureClient(clientOptions);

        return builder.AddVault(source =>
        {
            source.ClientOptions = clientOptions;
            source.MountPath = mountPath;
            source.Path = path;
            source.Version = version;
        });
    }

    /// <summary>
    ///     Добавляет источник конфигурации на основе секретов HashicorpVault, с заданными параметрами.
    /// </summary>
    /// <remarks>
    ///     Добавленный поставщик, загружает последнюю доступную версию, указанного секрета,
    ///     и если определён <paramref name="reloadInterval"/>, отслеживает появление новых версий, и загружает их.
    /// </remarks>
    /// <param name="builder">Ссылка на объект <see cref="IConfigurationBuilder" />.</param>
    /// <param name="configureClient">Действие, которое настраивает <see cref="VaultClientOptions" />.</param>
    /// <param name="path">Путь к секрету, относительный к <paramref name="mountPath" />.</param>
    /// <param name="reloadInterval">Версия, которую следует загрузить при первоначальной загрузке.</param>
    /// <param name="mountPath">Путь к ранилищу секретов. Значение по умолчанию "kv/".</param>
    /// <returns>Ссылка на объект <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddVault(
        this IConfigurationBuilder builder,
        Action<VaultClientOptions> configureClient,
        string path,
        TimeSpan? reloadInterval = null,
        string mountPath = "kv/")
    {
        var clientOptions = new VaultClientOptions();

        configureClient(clientOptions);

        return builder.AddVault(source =>
        {
            source.ClientOptions = clientOptions;
            source.MountPath = mountPath;
            source.Path = path;
            source.ReloadInterval = reloadInterval;
        });
    }

    /// <summary>
    ///     Добавляет источник конфигурации HashicorpVault.
    /// </summary>
    /// <param name="builder">Ссылка на объект <see cref="IConfigurationBuilder" />.</param>
    /// <param name="configure">Действие, которое настраивает <see cref="VaultConfigurationSource" />.</param>
    /// <returns>Ссылка на объект <see cref="IConfigurationBuilder" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Возникает, если значение параметра <paramref name="builder" /> = <see langword="null" />.
    /// </exception>
    public static IConfigurationBuilder AddVault(this IConfigurationBuilder builder,
        Action<VaultConfigurationSource>? configure)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.Add(configure);
    }
}