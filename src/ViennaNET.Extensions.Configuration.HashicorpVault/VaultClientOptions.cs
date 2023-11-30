using Microsoft.Extensions.Configuration;

namespace ViennaNET.Extensions.Configuration.HashicorpVault;

/// <summary>
///     Параметры клиента <see cref="VaultSharp.IVaultClient" />.
/// </summary>
public class VaultClientOptions
{
    /// <summary>
    ///     Базовый адрес узла Vault.
    /// </summary>
    /// <example>http://127.0.0.1:8200</example>
    [ConfigurationKeyName("VAULT_BASE_ADDRESS")]
    public string BaseAddress { get; set; }

    /// <summary>
    ///     Идентификатор клиента (приложения).
    /// </summary>
    [ConfigurationKeyName("VAULT_APP_ROLE_ID")]
    public string AppRoleId { get; set; }

    /// <summary>
    ///     Секрет клиента (приложения).
    /// </summary>
    [ConfigurationKeyName("VAULT_APP_SECRET_ID")]
    public string? AppSecretId { get; set; }
}