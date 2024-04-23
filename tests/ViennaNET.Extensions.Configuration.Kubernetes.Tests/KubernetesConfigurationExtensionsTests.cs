using Microsoft.Extensions.Configuration;

namespace ViennaNET.Extensions.Configuration.Kubernetes.Tests;

public class KubernetesConfigurationExtensionsTests
{
    [Test]
    public void AddJsonConfigMap_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddJsonConfigMap("appsettings.json", "default"), Is.Not.Null);
    }

    [Test]
    public void AddJsonConfigMap_WithFileName_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddJsonConfigMap("appsettings.json", "default", "appsettings.json"), Is.Not.Null);
    }

    [Test]
    public void AddKeyValueConfigMap_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddKeyValueConfigMap("appsettings", "default"), Is.Not.Null);
    }

    [Test]
    public void AddKeyValueConfigMap_WithReloadDisable_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddKeyValueConfigMap("appsettings", "default", false), Is.Not.Null);
    }

    [Test]
    public void AddJsonSecret_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddJsonSecret("appsecrets.json", "default"), Is.Not.Null);
    }

    [Test]
    public void AddJsonSecret_WithFileName_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddJsonSecret("appsecrets.json", "default", "appsecrets.json"), Is.Not.Null);
    }

    [Test]
    public void AddKeyValueSecret_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddKeyValueSecret("appsecrets", "default"), Is.Not.Null);
    }

    [Test]
    public void AddKeyValueSecret_WithReloadDisable_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddKeyValueConfigMap("appsecrets", "default", false), Is.Not.Null);
    }

    [Test]
    public void AddKubernetes_Throws_ArgumentNullException()
    {
        Assert.That(() => ((ConfigurationBuilder)null!).AddKubernetes(_ => { }), Throws.ArgumentNullException);
    }

    [Test]
    public void AddKeyValueConfigMap_NoNamespace_Returns_ConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();

        Assert.That(builder.AddKeyValueConfigMap("appsettings"), Is.Not.Null);
    }
}