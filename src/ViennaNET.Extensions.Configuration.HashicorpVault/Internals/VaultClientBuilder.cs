using System.Diagnostics.CodeAnalysis;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;

namespace ViennaNET.Extensions.Configuration.HashicorpVault.Internals;

[ExcludeFromCodeCoverage(Justification = "Не содержит логики, которую следовало бы протестировать.")]
internal class VaultClientBuilder : IVaultClientBuilder
{
    public IVaultClient Build(VaultClientOptions clientOptions)
    {
        return new VaultClient(new VaultClientSettings(
            clientOptions.BaseAddress,
            new AppRoleAuthMethodInfo(clientOptions.AppRoleId, clientOptions.AppSecretId)));
    }
}