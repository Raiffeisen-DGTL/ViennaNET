using System.Text.Json;
using System.Text.Json.Nodes;
using Moq;
using VaultSharp.V1.Commons;

namespace ViennaNET.Extensions.Configuration.HashicorpVault.Tests;

public class VaultConfigurationProviderTests
{
    private static JsonDocumentOptions DocOptions = new() { AllowTrailingCommas = true };

    [Test]
    public void Ctor_WithNullSource_Throws_ArgumentNullException()
    {
        var clientBuilder = Mock.Of<IVaultClientBuilder>();

        Assert.That(() => new VaultConfigurationProvider(null!, clientBuilder), Throws.ArgumentNullException);
    }

    [Test]
    public void Ctor_WithNullBuilder_Throws_ArgumentNullException()
    {
        var source = new VaultConfigurationSource();

        Assert.That(() => new VaultConfigurationProvider(source, null!), Throws.ArgumentNullException);
    }

    [Test]
    public void Dispose_Throws_Nothing()
    {
        var provider =
            new VaultConfigurationProvider(new VaultConfigurationSource(), Mock.Of<IVaultClientBuilder>());

        Assert.That(() => provider.Dispose(), Throws.Nothing);
    }

    [Test]
    public void Dispose_CanBeCalled_MultipleTimes()
    {
        var clientBuilder = Mock.Of<IVaultClientBuilder>();
        var source = new VaultConfigurationSource();
        var provider = new VaultConfigurationProvider(source, clientBuilder);

        provider.Dispose();

        Assert.That(() => provider.Dispose(), Throws.Nothing);
    }

    [Test]
    public void LoadAsync_Returns_ReloadedData()
    {
        var secret = new SecretData
        {
            Data = JsonNode.Parse(Given.AppSettingsJsonV2, documentOptions: DocOptions)?.AsObject()
                .ToDictionary(pair => pair.Key, pair => pair.Value as object),
            Metadata = new CurrentSecretMetadata { Version = 1 }
        };
        var client = Given.GetClientMockWithSetupReadSecretAsync(secret).Object;
        var builder = Mock.Of<IVaultClientBuilder>(clientBuilder =>
            clientBuilder.Build(It.IsAny<VaultClientOptions>()) == client);
        var source = Given.GetSource();
        var provider = new VaultConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(provider.TryGet("RootIntProp", out _), Is.True);
    }

    [Test]
    public void LoadAsync_Call_PollAsync_Throws_Nothing()
    {
        var secret = new SecretData
        {
            Data = JsonNode.Parse(Given.AppSettingsJsonV2, documentOptions: DocOptions)?.AsObject()
                .ToDictionary(pair => pair.Key, pair => pair.Value as object),
            Metadata = new CurrentSecretMetadata { Version = 1 }
        };
        var client = Given.GetClientMockWithSetupReadSecretAsync(secret).Object;
        var builder = Mock.Of<IVaultClientBuilder>(clientBuilder =>
            clientBuilder.Build(It.IsAny<VaultClientOptions>()) == client);
        var source = Given.GetSource(reloadInterval: TimeSpan.FromSeconds(1));
        var provider = new VaultConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(async () =>
        {
            provider.Load();
            await Task.Delay(source.ReloadInterval!.Value);
        }, Throws.Nothing);
    }

    [Test]
    public async Task LoadAsync_Call_PollAsync_Call_OnPollException()
    {
        var secret = new SecretData
        {
            Data = JsonNode.Parse(Given.AppSettingsJsonV2, documentOptions: DocOptions)?.AsObject()
                .ToDictionary(pair => pair.Key, pair => pair.Value as object),
            Metadata = new CurrentSecretMetadata { Version = 1 }
        };
        var client = Given.GetClientMockWithSetupReadSecretAsync(secret).Object;
        var builder = Mock.Of<IVaultClientBuilder>(clientBuilder =>
            clientBuilder.Build(It.IsAny<VaultClientOptions>()) == client);
        var source = Given.GetSource(reloadInterval: TimeSpan.FromSeconds(1));
        var provider = new VaultConfigurationProvider(source, builder);

        source.OnPollException += exception => Assert.That(exception, Is.TypeOf<InvalidOperationException>());

        provider.Load();
        provider.Load();
        await Task.Delay(source.ReloadInterval!.Value + TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task LoadAsync_Call_PollAsync_Canceled()
    {
        var secret = new SecretData
        {
            Data = JsonNode.Parse(Given.AppSettingsJsonV2, documentOptions: DocOptions)?.AsObject()
                .ToDictionary(pair => pair.Key, pair => pair.Value as object),
            Metadata = new CurrentSecretMetadata { Version = 1 }
        };
        var client = Given.GetClientMockWithSetupReadSecretAsync(secret).Object;
        var builder = Mock.Of<IVaultClientBuilder>(clientBuilder =>
            clientBuilder.Build(It.IsAny<VaultClientOptions>()) == client);
        var source = Given.GetSource(reloadInterval: TimeSpan.FromSeconds(1));
        var provider = new VaultConfigurationProvider(source, builder);

        provider.Load();
        provider.Dispose();
        await Task.Delay(source.ReloadInterval!.Value + TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task LoadAsync_Call_PollAsync_Reload()
    {
        var firstSecret = new SecretData
        {
            Data = JsonNode.Parse(Given.AppSettingsJsonV1, documentOptions: DocOptions)?.AsObject()
                .ToDictionary(pair => pair.Key, pair => pair.Value as object),
            Metadata = new CurrentSecretMetadata { Version = 1 }
        };
        var secondSecret = new SecretData
        {
            Data = JsonNode.Parse(Given.AppSettingsJsonV2, documentOptions: DocOptions)?.AsObject()
                .ToDictionary(pair => pair.Key, pair => pair.Value as object),
            Metadata = new CurrentSecretMetadata { Version = 2 }
        };
        var client = Given.GetClientMockWithSetupReadSecretAsync(firstSecret, secondSecret).Object;
        var builder = Mock.Of<IVaultClientBuilder>(clientBuilder =>
            clientBuilder.Build(It.IsAny<VaultClientOptions>()) == client);
        var source = Given.GetSource(reloadInterval: TimeSpan.FromSeconds(1));
        var provider = new VaultConfigurationProvider(source, builder);

        provider.Load();
        await Task.Delay(source.ReloadInterval!.Value + TimeSpan.FromSeconds(1));

        provider.TryGet("ConnectionStrings:TestDB", out var connectionString);

        Assert.That(connectionString, Is.EqualTo("Server=(localdb)\\mssqllocaldbv2;Database=TestDB;"));
    }

    [Test]
    public async Task LoadAsync_Call_PollAsync_Not_Once()
    {
        var firstSecret = new SecretData
        {
            Data = JsonNode.Parse(Given.AppSettingsJsonV1, documentOptions: DocOptions)?.AsObject()
                .ToDictionary(pair => pair.Key, pair => pair.Value as object),
            Metadata = new CurrentSecretMetadata { Version = 1 }
        };
        var secondSecret = new SecretData
        {
            Data = JsonNode.Parse(Given.AppSettingsJsonV2, documentOptions: DocOptions)?.AsObject()
                .ToDictionary(pair => pair.Key, pair => pair.Value as object),
            Metadata = new CurrentSecretMetadata { Version = 2 }
        };
        var client = Given.GetClientMockWithSetupReadSecretAsync(firstSecret, secondSecret).Object;
        var builder = Mock.Of<IVaultClientBuilder>(clientBuilder =>
            clientBuilder.Build(It.IsAny<VaultClientOptions>()) == client);
        var source = Given.GetSource();
        var provider = new VaultConfigurationProvider(source, builder);

        provider.Load();

        source.ReloadInterval = TimeSpan.FromSeconds(1);
        provider.Dispose();
        provider.Load();

        await Task.Delay(source.ReloadInterval!.Value + TimeSpan.FromSeconds(1));
    }

    [Test]
    public void LoadAsync_Destroyed_Throws_Nothing()
    {
        var firstSecret = new SecretData
        {
            Data = JsonNode.Parse(Given.AppSettingsJsonV1, documentOptions: DocOptions)?.AsObject()
                .ToDictionary(pair => pair.Key, pair => pair.Value as object),
            Metadata = new CurrentSecretMetadata { Version = 1, Destroyed = true }
        };
        var client = Given.GetClientMockWithSetupReadSecretAsync(firstSecret).Object;
        var builder = Mock.Of<IVaultClientBuilder>(clientBuilder =>
            clientBuilder.Build(It.IsAny<VaultClientOptions>()) == client);
        var source = Given.GetSource(reloadInterval: TimeSpan.FromSeconds(1));
        var provider = new VaultConfigurationProvider(source, builder);

        Assert.That(() => provider.Load(), Throws.Nothing);
    }
}