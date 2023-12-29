using Microsoft.Extensions.Configuration;
using Moq;

namespace ViennaNET.Extensions.Configuration.Kubernetes.Tests;

public class KubernetesConfigurationSourceTests
{
    [SetUp]
    public void SetUp()
    {
        // Заставляем KubernetesClientBuilder думать, буд-то приложение запущено в кластере
        // И получать соответствующую конфигурацию (заглушку).
        Environment.SetEnvironmentVariable("KUBERNETES_SERVICE_HOST", "test.kube.api/");
        Environment.SetEnvironmentVariable("KUBERNETES_SERVICE_PORT", "8080");
    }

    [Test]
    public void Ctor_DoesNotThrows()
    {
        Assert.That(() => new KubernetesConfigurationSource(), Throws.Nothing);
    }

    [TestCase(null, null, null, true)]
    [TestCase("./test.kubeconfig", "testcontext", "https://test.kube.api", false)]
    public void ParameterizedCtor_DoesNotThrows(string? configPath, string? сontext, string? masterUrl,
        bool useRelativePaths)
    {
        Assert.That(() => new KubernetesConfigurationSource(), Throws.Nothing);
    }

    [Test]
    public void Ctor_Init_DefaultValues()
    {
        var source = new KubernetesConfigurationSource();

        Assert.Multiple(() =>
        {
            Assert.That(source.Namespace, Is.EqualTo("default"));
            Assert.That(source.Name, Is.EqualTo("appsettings"));
            Assert.That(source.DataType, Is.EqualTo(DataTypes.Json));
            Assert.That(source.ReloadOnChange, Is.EqualTo(false));
            Assert.That(source.Kind, Is.EqualTo(Kinds.ConfigMap));
            Assert.That(source.FileName, Is.Null);
            Assert.That(source.OnWatchClose, Is.Null);
            Assert.That(source.OnWatchException, Is.Null);
        });
    }

    [Test]
    public void Initializer_Callbacks_And_FileName_Initialized()
    {
        var source = new KubernetesConfigurationSource()
        {
            FileName = "test.json",
            OnWatchClose = () => TestContext.WriteLine("WatchCloseTest"),
            OnWatchException = TestContext.WriteLine
        };

        Assert.Multiple(() =>
        {
            Assert.That(source.FileName, Is.EqualTo("test.json"));
            Assert.That(source.OnWatchClose, Is.Not.Null);
            Assert.That(source.OnWatchException, Is.Not.Null);
        });
    }

    [TestCase(Kinds.ConfigMap)]
    [TestCase(Kinds.Secret)]
    public void Build_Returns_ConfigurationProvider(int kind)
    {
        var builder = Mock.Of<IConfigurationBuilder>();
        var source = new KubernetesConfigurationSource
        {
            FileName = "test.json",
            Kind = (Kinds)kind,
            OnWatchClose = () => TestContext.WriteLine("WatchCloseTest"),
            OnWatchException = TestContext.WriteLine,
        };

        Assert.That(source.Build(builder),
            (Kinds)kind == Kinds.ConfigMap
                ? Is.Not.Null.And.AssignableFrom(typeof(ConfigMapConfigurationProvider))
                : Is.Not.Null.And.AssignableFrom(typeof(SecretConfigurationProvider)));
    }

    [Test]
    public void Build_Throws_ArgumentOutOfRangeException()
    {
        var builder = Mock.Of<IConfigurationBuilder>();
        var source = new KubernetesConfigurationSource
        {
            FileName = "test.json",
            Kind = (Kinds)3,
            OnWatchClose = () => TestContext.WriteLine("WatchCloseTest"),
            OnWatchException = TestContext.WriteLine
        };

        Assert.That(() => source.Build(builder), Throws.InvalidOperationException);
    }
}