using Microsoft.Extensions.Configuration;
using Moq;

namespace ViennaNET.Extensions.Configuration.HashicorpVault.Tests;

public class VaultConfigurationExtensionsTests
{
    private const string Address = "https://tst.raif.ru";
    private const string Path = "https://tst.raif.ru";

    [Test]
    public void AddVault_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddVault(options => options.BaseAddress = Address, Path, 1), Is.Not.Null);
    }

    [Test]
    public void AddVault_WithReloadInterval_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(
            builder.AddVault(options => options.BaseAddress = Address, Path, TimeSpan.FromDays(1)), Is.Not.Null);
    }

    [Test]
    public void AddVault_Throws_ArgumentNullException()
    {
        Assert.That(() => default(ConfigurationBuilder)!.AddVault(It.IsAny<Action<VaultConfigurationSource>>()),
            Throws.ArgumentNullException);
    }
}