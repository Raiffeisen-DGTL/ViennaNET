using VaultSharp;

namespace ViennaNET.Extensions.Configuration.HashicorpVault;

/// <summary>
///     Сборщик клиента API Hashicorp Vault.
/// </summary>
public interface IVaultClientBuilder
{
    /// <summary>
    ///     Создаёт и инициализирует клиент API Hashicorp Vault <see cref="IVaultClient" />.
    /// </summary>
    /// <param name="clientOptions">Ссылка на объект <see cref="VaultClientOptions" />.</param>
    /// <returns>Ссылка на объект <see cref="IVaultClient" />.</returns>
    public IVaultClient Build(VaultClientOptions clientOptions);
}