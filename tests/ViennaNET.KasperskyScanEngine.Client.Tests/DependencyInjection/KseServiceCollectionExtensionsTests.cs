using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ViennaNET.KasperskyScanEngine.Client.DependencyInjection;

namespace ViennaNET.KasperskyScanEngine.Client.Tests.DependencyInjection;

public class KseServiceCollectionExtensionsTests
{
    private ConfigurationBuilder _builder = null!;
    private IServiceCollection _services = null!;

    [SetUp]
    public void Setup()
    {
        _builder = new ConfigurationBuilder();
        _services = new ServiceCollection();

        List<KeyValuePair<string, string?>> data =
        [
            new KeyValuePair<string, string?>("Endpoints:KasperskyScanEngine:BaseAddress", "http://localhost:10201/"),
            new KeyValuePair<string, string?>("Endpoints:KasperskyScanEngine:OverallTimeout", "10"),
            new KeyValuePair<string, string?>("Endpoints:KasperskyScanEngine:TryTimeout", "2")
        ];

        _builder.AddInMemoryCollection(data);
    }

    [Test]
    [Category("Integration")]
    public void AddKasperskyScanEngineApi_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => _services.AddKasperskyScanEngineApi(_builder.Build()));
    }

    [Test]
    [Category("Integration")]
    public void AddKasperskyScanEngineApi_JsonSerializerOptions_Resolved()
    {
        _services.AddKasperskyScanEngineApi(_builder.Build());

        var options = _services.BuildServiceProvider()
            .GetRequiredService<IOptionsSnapshot<JsonSerializerOptions>>()
            .Get(IKasperskyScanEngineApi.JsonSerializerOptionsName);

        Assert.Multiple(() =>
        {
            Assert.That(options, Is.Not.Null);
            Assert.That(options.WriteIndented, Is.False);
            Assert.That(options.AllowTrailingCommas, Is.False);
            Assert.That(options.DefaultIgnoreCondition, Is.EqualTo(JsonIgnoreCondition.WhenWritingNull));
            Assert.That(options.TypeInfoResolver, Is.EqualTo(KseClientSerializerContext.Default));
        });
    }

    [Test]
    [Category("Integration")]
    public void AddKasperskyScanEngineApi_ConfigurePrimaryHttpMessageHandler_Return_DefaultPrimaryHandler()
    {
        _services.AddKasperskyScanEngineApi(_builder.Build());

        var api = _services.BuildServiceProvider().GetRequiredService<IKasperskyScanEngineApi>();

        Assert.That(api, Is.Not.Null);
    }

    [Test]
    [Category("Integration")]
    public void AddKasperskyScanEngineApi_ConfigurePrimaryHttpMessageHandler_Return_UnixSocketHandler()
    {
        _builder.AddInMemoryCollection(
            [new KeyValuePair<string, string?>("Endpoints:KasperskyScanEngine:AuthorizationToken", 
                "SldYQTUyOUNVMnE3VWR2N3Izamk2QkVNc2hhLTV5dTBLcVUzeXZLdGYtNkkrVFUyQUVRQUNLQUFCSWdwRUlJTQ")]);
        _services.AddKasperskyScanEngineApi(_builder.Build());

        var api = _services.BuildServiceProvider().GetRequiredService<IKasperskyScanEngineApi>();

        Assert.That(api, Is.Not.Null);
    }
}