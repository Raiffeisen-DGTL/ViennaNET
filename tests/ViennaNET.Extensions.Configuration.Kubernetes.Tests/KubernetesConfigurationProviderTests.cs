using ViennaNET.Extensions.Configuration.Kubernetes.Internals;

namespace ViennaNET.Extensions.Configuration.Kubernetes.Tests;

public class KubernetesConfigurationProviderTests
{
    [Test]
    public void Ctor_WithNullSource_Throws_ArgumentNullException()
    {
        Assert.That(() => new ConfigMapConfigurationProvider(null!, new KubernetesClientBuilder()),
            Throws.ArgumentNullException);
    }

    [Test]
    public void Ctor_WithNullBuilder_Throws_ArgumentNullException()
    {
        Assert.That(() => new ConfigMapConfigurationProvider(new KubernetesConfigurationSource(), null!),
            Throws.ArgumentNullException);
    }
}