using System.Text;
using k8s;
using k8s.Models;
using Moq;
using ViennaNET.Extensions.Configuration.Kubernetes.Internals;

namespace ViennaNET.Extensions.Configuration.Kubernetes.Tests;

public class SecretConfigurationProviderTests
{
    [Test]
    public void Ctor_Throws_Nothing()
    {
        var builder = Mock.Of<IKubernetesClientBuilder>();
        var source = Given.GetSource(Kinds.Secret, DataTypes.Json, false, Given.FileName);

        Assert.That(() => new SecretConfigurationProvider(source, builder), Throws.Nothing);
    }

    [Test]
    public void Load_Single_Parse_TestJson_Returns_TestOption()
    {
        var secret = new V1Secret
        {
            Kind = "Secret",
            Type = "kubernetes.io/appsecretsjson",
            Metadata = Given.Meta,
            Data = new Dictionary<string, byte[]>
            {
                { Given.FileName, Encoding.UTF8.GetBytes(Given.FileJsonContent) }
            }
        };
        var client = Given.GetClientMock(secret).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.Secret, DataTypes.Json, false, Given.FileName);
        var provider = new SecretConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(provider.TryGet("TestOption", out _), Is.True);
    }

    [Test]
    public void Load_Single_Parse_TestJson_Throws_FormatException()
    {
        var secret = new V1Secret
        {
            Kind = "Secret",
            Type = "kubernetes.io/appsecretsjson",
            Metadata = Given.Meta,
            Data = new Dictionary<string, byte[]>
            {
                { Given.FileName, Encoding.UTF8.GetBytes(Given.InvalidFileJsonContent) }
            }
        };
        var client = Given.GetClientMock(secret).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.Secret, DataTypes.Json, false, Given.FileName);
        var provider = new SecretConfigurationProvider(source, builder);

        Assert.That(() => provider.Load(), Throws.TypeOf<FormatException>());
    }

    [Test]
    public void Load_Single_WithSecret_WithoutType_Returns_Empty()
    {
        var secret = new V1Secret
        {
            Kind = "Secret",
            Type = string.Empty,
            Metadata = Given.Meta,
            Data = new Dictionary<string, byte[]>
            {
                { Given.FileName, Encoding.UTF8.GetBytes(Given.FileJsonContent) }
            }
        };
        var client = Given.GetClientMock(secret).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.Secret, DataTypes.Json, false, Given.FileName);
        var provider = new SecretConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(provider.TryGet("TestOption", out _), Is.False);
    }

    [Test]
    public void Load_Single_Parse_KeyValue_Throws_FormatException()
    {
        var secret = new V1Secret
        {
            Kind = "Secret",
            Metadata = Given.Meta,
            Data = new Dictionary<string, byte[]> { { "TestOption:123", Encoding.UTF8.GetBytes("TestOptValue") } }
        };
        var client = Given.GetClientMock(secret).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.Secret, DataTypes.KeyValue, false);
        var provider = new SecretConfigurationProvider(source, builder);

        Assert.That(() => provider.Load(), Throws.TypeOf<FormatException>());
    }

    [Test]
    public void Load_Single_Parse_KeyValue_Returns_TestOption()
    {
        var secret = new V1Secret
        {
            Kind = "Secret",
            Metadata = Given.Meta,
            Data = new Dictionary<string, byte[]> { { "TestOption", Encoding.UTF8.GetBytes("TestOptValue") } }
        };
        var client = Given.GetClientMock(secret).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.Secret, DataTypes.KeyValue, false);
        var provider = new SecretConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(provider.TryGet("TestOption", out _), Is.True);
    }

    [Test]
    public void Load_Watch_Returns_Watcher()
    {
        var secretList = new V1SecretList
        {
            Items = new List<V1Secret>
            {
                new()
                {
                    Kind = "Secret",
                    Metadata = Given.Meta,
                    Data = new Dictionary<string, byte[]>
                    {
                        { "TestOption", Encoding.UTF8.GetBytes("TestOptValue") }
                    }
                }
            }
        };
        var client = Given.GetClientMock(secretList).WithV1Secret(secretList.Items.First()).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.Secret, DataTypes.KeyValue, true);
        var provider = new SecretConfigurationProvider(source, builder);

        provider.Load();

        Assert.That(provider.Watcher, Is.Not.Null);
    }

    [Test]
    public void OnEvent_Type_Not_Modified_Or_Added_Returns()
    {
        var secret = new V1Secret
        {
            Kind = "Secret",
            Metadata = Given.Meta,
            Data = new Dictionary<string, byte[]> { { "TestOption", Encoding.UTF8.GetBytes("TestOptValue") } }
        };
        var client = Given.GetClientMock(secret).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.Secret, DataTypes.KeyValue, false);
        var provider = new SecretConfigurationProvider(source, builder);

        provider.OnEvent(WatchEventType.Bookmark, secret);

        Assert.That(provider.TryGet("TestOption", out _), Is.False);
    }

    [TestCase(WatchEventType.Added)]
    [TestCase(WatchEventType.Modified)]
    public void OnEvent_Type_Call_Extract(WatchEventType eventType)
    {
        var secret = new V1Secret
        {
            Kind = "Secret",
            Metadata = Given.Meta,
            Data = new Dictionary<string, byte[]> { { "TestOption", Encoding.UTF8.GetBytes("TestOptValue") } }
        };
        var client = Given.GetClientMock(secret).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.Secret, DataTypes.KeyValue, false);
        var provider = new SecretConfigurationProvider(source, builder);

        provider.OnEvent(eventType, secret);

        Assert.That(provider.TryGet("TestOption", out _), Is.True);
    }

    [Test]
    public void OnEvent_Extract_WithDefaultFileName_Returns_TestOption()
    {
        var secret = new V1Secret
        {
            Kind = "Secret",
            Type = "kubernetes.io/appsecretsjson",
            Metadata = Given.Meta,
            Data = new Dictionary<string, byte[]>
            {
                { "appsecrets.json", Encoding.UTF8.GetBytes(Given.FileJsonContent) }
            }
        };
        var client = Given.GetClientMock(secret).Object;
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var source = Given.GetSource(Kinds.Secret, DataTypes.Json, false);

        var provider = new SecretConfigurationProvider(source, builder);

        provider.OnEvent(WatchEventType.Modified, secret);

        Assert.That(provider.TryGet("TestOption", out _), Is.True);
    }

    [Test]
    public void Dispose_Throws_Nothing()
    {
        var client = Mock.Of<IKubernetes>();
        var builder = Mock.Of<IKubernetesClientBuilder>(clientBuilder => clientBuilder.Build() == client);
        var provider = new SecretConfigurationProvider(new KubernetesConfigurationSource(), builder);

        Assert.That(provider.Dispose, Throws.Nothing);
    }
}