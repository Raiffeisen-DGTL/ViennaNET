using k8s;
using k8s.Models;
using Moq;

namespace ViennaNET.Extensions.Configuration.Kubernetes.Tests;

public class ConfigMapConfigurationProviderTests
{
    [Test]
    public void Ctor_Throws_Nothing()
    {
        var builder = Mock.Of<IKubernetesClientBuilder>();
        var source = Given.GetSource(Kinds.ConfigMap, DataTypes.Json, false, Given.FileName);

        Assert.That(() => new ConfigMapConfigurationProvider(source, builder), Throws.Nothing);
    }

    [Test]
    public void Load_Single_Parse_TestJson_Returns_TestOption()
    {
        var configMap = new V1ConfigMap
        {
            Kind = "ConfigMap",
            Metadata = Given.Meta,
            Data = new Dictionary<string, string> { { Given.FileName, Given.FileJsonContent } }
        };
        var client = Given.GetClientMock(configMap).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.ConfigMap, DataTypes.Json, false, Given.FileName);

        var provider = new ConfigMapConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(provider.TryGet("TestOption", out _), Is.True);
    }

    [Test]
    public void Load_Single_Returns_Empty()
    {
        //Ожидаем файл с именем Given.FileName, а возвращается Given.OtherFileName.

        var configMap = new V1ConfigMap
        {
            Kind = "ConfigMap",
            Metadata = Given.Meta,
            Data = new Dictionary<string, string> { { Given.OtherFileName, Given.FileJsonContent } }
        };
        var client = Given.GetClientMock(configMap).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.ConfigMap, DataTypes.Json, false, Given.FileName);

        var provider = new ConfigMapConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(provider.TryGet("TestOption", out _), Is.False);
    }

    [Test]
    public void Load_Single_Parse_KeyValue_Returns_TestOption()
    {
        var configMap = new V1ConfigMap
        {
            Kind = "ConfigMap",
            Metadata = Given.Meta,
            Data = new Dictionary<string, string> { { "TestOption", "TestOptValue" } }
        };
        var client = Given.GetClientMock(configMap).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.ConfigMap, DataTypes.KeyValue, false);

        var provider = new ConfigMapConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(provider.TryGet("TestOption", out _), Is.True);
    }

    [Test]
    public void Load_Watch_Returns_Watcher()
    {
        var configMapList = new V1ConfigMapList
        {
            Items = new List<V1ConfigMap>
            {
                new()
                {
                    Kind = "ConfigMap",
                    Metadata = Given.Meta,
                    Data = new Dictionary<string, string> { { "TestOption", "TestOptValue" } }
                }
            }
        };
        var client = Given.GetClientMock(configMapList).WithV1ConfigMap(configMapList.Items.First()).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.ConfigMap, DataTypes.KeyValue, true);

        var provider = new ConfigMapConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(provider.Watcher, Is.Not.Null);
    }

    [Test]
    public void OnEvent_Type_Not_Modified_Or_Added_Returns()
    {
        var configMap = new V1ConfigMap
        {
            Kind = "ConfigMap",
            Metadata = Given.Meta,
            Data = new Dictionary<string, string> { { "TestOption", "TestOptValue" } }
        };
        var client = Given.GetClientMock(configMap).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.ConfigMap, DataTypes.KeyValue, false);

        var provider = new ConfigMapConfigurationProvider(source, builder);

        provider.OnEvent(WatchEventType.Bookmark, configMap);

        Assert.That(provider.TryGet("TestOption", out _), Is.False);
    }

    [TestCase(WatchEventType.Added)]
    [TestCase(WatchEventType.Modified)]
    public void OnEvent_Type_Call_Extract(WatchEventType eventType)
    {
        var configMap = new V1ConfigMap
        {
            Kind = "ConfigMap",
            Metadata = Given.Meta,
            Data = new Dictionary<string, string> { { "TestOption", "TestOptValue" } }
        };
        var client = Given.GetClientMock(configMap).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.ConfigMap, DataTypes.KeyValue, false);

        var provider = new ConfigMapConfigurationProvider(source, builder);

        provider.OnEvent(eventType, configMap);

        Assert.That(provider.TryGet("TestOption", out _), Is.True);
    }

    [Test]
    public void OnEvent_Extract_WithDefaultFileName_Returns_TestOption()
    {
        var configMap = new V1ConfigMap
        {
            Kind = "ConfigMap",
            Metadata = Given.Meta,
            Data = new Dictionary<string, string> { { "appsettings.json", Given.FileJsonContent } }
        };
        var client = Given.GetClientMock(configMap).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.ConfigMap, DataTypes.Json, false);

        var provider = new ConfigMapConfigurationProvider(source, builder);

        provider.OnEvent(WatchEventType.Modified, configMap);

        Assert.That(provider.TryGet("TestOption", out _), Is.True);
    }

    [Test]
    public void Dispose_Throws_Nothing()
    {
        var k8sClient = Mock.Of<IKubernetes>();
        var k8sClientBuilder = new Mock<IKubernetesClientBuilder>();

        k8sClientBuilder.Setup(builder => builder.Build()).Returns(k8sClient);
        
        var provider =
            new ConfigMapConfigurationProvider(new KubernetesConfigurationSource(), k8sClientBuilder.Object);

        Assert.That(() => provider.Dispose(), Throws.Nothing);
    }
}