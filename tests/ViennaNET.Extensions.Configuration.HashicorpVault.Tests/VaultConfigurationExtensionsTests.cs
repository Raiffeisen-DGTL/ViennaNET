using Microsoft.Extensions.Configuration;

namespace ViennaNET.Extensions.Configuration.HashicorpVault.Tests;

public class VaultConfigurationExtensionsTests
{
    [Test]
    public void AddVault_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddVault(options => options.BaseAddress = "https://tst.raif.ru", "tst", 1), Is.Not.Null);
    }

    [Test]
    public void AddVault_WithReloadInterval_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(
            builder.AddVault(options => options.BaseAddress = "https://tst.raif.ru", "tst", TimeSpan.FromDays(1)),
            Is.Not.Null);
    }
    
    [Test]
    public void AddVault_Throws_ArgumentNullException()
    {
        var builder = (ConfigurationBuilder)null!;

        Assert.That(() =>
            builder.AddVault(options => options.BaseAddress = "https://tst.raif.ru", "tst", TimeSpan.FromDays(1)),
            Throws.ArgumentNullException);
    }
}