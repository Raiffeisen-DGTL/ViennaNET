using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using ViennaNET.Extensions.Http.Authentication;
using ViennaNET.Extensions.Http.DependencyInjection;

namespace ViennaNET.Extensions.Http.Tests.Authentication;

public class AuthenticationHttpClientBuilderExtensionsTests
{
    private IConfigurationBuilder _builder = null!;
    private IServiceCollection _services = null!;

    [SetUp]
    public void Setup()
    {
        _builder = new ConfigurationBuilder();
        _services = new ServiceCollection();

        _builder.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "Endpoints:TestApi:BaseAddress", "http://test.raif.ru:10201/" },
            { "Endpoints:TestApi:Authentication:Negotiate:UseDefaultCredentials", "true" }
        });

        _services.Configure<TestHttpClientOptions>(_builder.Build().GetSection(TestHttpClientOptions.SectionName));
    }

    [Test]
    public void UseNegotiateAuthentication_Configure_UseDefaultCredentials()
    {
        var builder = _services
            .AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>()
            .UseNegotiateAuthentication<TestHttpClientOptions>();
        var provider = _services.BuildServiceProvider(true);
        var actions = provider.GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>().Get(builder.Name)
            .HttpMessageHandlerBuilderActions;

        Assert.Multiple(() =>
        {
            Assert.That(actions.Any(action => action.Method.Name.Contains("ConfigurePrimaryHttpMessageHandler")),
                Is.True);
            Assert.That(provider.GetRequiredService<ITestHttpClient>(), Is.Not.Null);
        });
    }

    [Test]
    public void UseNegotiateAuthentication_Configure_UseUserName_And_Password()
    {
        _builder.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "Endpoints:TestApi:BaseAddress", "http://test.raif.ru:10201/" },
            { "Endpoints:TestApi:Authentication:Negotiate:UseDefaultCredentials", "false" },
            { "Endpoints:TestApi:Authentication:Negotiate:UserName", "ruatst1" },
            { "Endpoints:TestApi:Authentication:Negotiate:Password", "tst12345" },
        });

        var builder = _services
            .AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>(options =>
                _builder.Build().GetSection(TestHttpClientOptions.SectionName).Bind(options))
            .UseNegotiateAuthentication<TestHttpClientOptions>();
        var provider = _services.BuildServiceProvider();
        var actions = provider.GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>().Get(builder.Name)
            .HttpMessageHandlerBuilderActions;

        Assert.Multiple(() =>
        {
            Assert.That(actions.Any(action => action.Method.Name.Contains("ConfigurePrimaryHttpMessageHandler")),
                Is.True);
            Assert.That(provider.GetRequiredService<ITestHttpClient>(), Is.Not.Null);
        });
    }

    [Test]
    public void UseNegotiateAuthentication_Throws_InvalidOperationException()
    {
        _services
            .AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>(options =>
                options.Authentication = null)
            .UseNegotiateAuthentication<TestHttpClientOptions>();
        var provider = _services.BuildServiceProvider();

        Assert.That(() => Assert.That(provider.GetRequiredService<ITestHttpClient>(), Is.Not.Null),
            Throws.InvalidOperationException);
    }
}