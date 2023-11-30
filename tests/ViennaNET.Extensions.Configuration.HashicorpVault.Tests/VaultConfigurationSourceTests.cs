using Microsoft.Extensions.Configuration;
using Moq;

namespace ViennaNET.Extensions.Configuration.HashicorpVault.Tests;

public class VaultConfigurationSourceTests
{
    [Test]
    public void Ctor_Throws_Nothing()
    {
        Assert.That(() => new VaultConfigurationSource(), Throws.Nothing);
    }
    
    [Test]
    public void Ctor_Init_DefaultValues()
    {
        var source = new VaultConfigurationSource();

        Assert.Multiple(() =>
        {
            Assert.That(source.ClientOptions, Is.Not.Null);
            Assert.That(source.MountPath, Is.EqualTo("kv/"));
            Assert.That(source.Path, Is.EqualTo("appsettings.json"));
            Assert.That(source.Version, Is.Null);
            Assert.That(source.ReloadInterval, Is.Null);
            Assert.That(source.OnPollException, Is.Null);
            Assert.That(source.ReloadOnChange, Is.False);
            Assert.That(source.VaultClientBuilder, Is.Not.Null);
        });
    }

    [Test]
    public void Build_Throws_Nothing()
    {
        var builder = Mock.Of<IConfigurationBuilder>();
        var source = new VaultConfigurationSource
        {
            ClientOptions = new VaultClientOptions
            {
                BaseAddress = "https://test.raiffeisen.ru:1234",
                AppRoleId = "6c43c870-5628-5e3e-4e04-17de57093960",
                AppSecretId = null
            }
        };

        Assert.That(() => source.Build(builder), Throws.Nothing);
    }
}