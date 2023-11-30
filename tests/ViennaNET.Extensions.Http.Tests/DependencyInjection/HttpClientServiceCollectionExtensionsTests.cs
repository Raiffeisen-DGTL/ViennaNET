using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using ViennaNET.Extensions.Http.DependencyInjection;
using ViennaNET.Extensions.Http.Logging;

namespace ViennaNET.Extensions.Http.Tests.DependencyInjection;

public class HttpClientServiceCollectionExtensionsTests
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
            { "Endpoints:TestApi:BaseAddress", "http://test.raif.ru:10201/" }
        });

        _services.Configure<TestHttpClientOptions>(_builder.Build().GetSection(TestHttpClientOptions.SectionName));
    }

    [Test]
    public void AddHttpClient_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => _services
            .AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>());
    }

    [Test]
    public void AddHttpClient_WithConfigure_TestHttpClientOptions()
    {
        Assert.DoesNotThrow(() => _services
            .AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>(options =>
            {
                options.RetryCount = 1;
                options.UseReplayPolicy = true;
            }));
    }

    [Test]
    public void AddLoggingHttpMessageBodyHandler_LoggingHttpMessageBodyHandler_Registered()
    {
        _services
            .AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>()
            .AddLoggingHttpMessageBodyHandler();

        var provider = _services.BuildServiceProvider(true);

        Assert.That(provider.GetRequiredService<LoggingHttpMessageBodyHandler>(), Is.Not.Null);
    }

    [Test]
    public async Task AddHttpClient_WithReplayPolicy_CallSendAsync_Twice()
    {
        var handler = new Mock<DelegatingHandler>(MockBehavior.Strict);
        var tryCount = 0;

        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway))
            .Callback(() => tryCount += 1)
            .Verifiable();

        _services.AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>()
            .AddHttpMessageHandler(() => handler.Object);

        _services.Configure<TestHttpClientOptions>(opt =>
        {
            opt.UseReplayPolicy = true;
            opt.UseReplayPolicyOnlyIdempotentRequest = false;
            opt.RetryCount = 1;
            opt.RetryDelay = 2;
        });

        var client =
            _services.BuildServiceProvider().GetRequiredService<ITestHttpClient>();

        await client.HttpClient.GetAsync("");

        Assert.That(tryCount, Is.EqualTo(2));
    }

    [Test]
    public async Task AddHttpClient_DisablePolicies_NoOpHandled()
    {
        var handler = new Mock<DelegatingHandler>(MockBehavior.Strict);
        var tryCount = 0;

        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway))
            .Callback(() => tryCount += 1)
            .Verifiable();

        _services.AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>()
            .AddHttpMessageHandler(() => handler.Object);

        _services.Configure<TestHttpClientOptions>(opt =>
        {
            opt.UseReplayPolicy = false;
            opt.UseReplayPolicyOnlyIdempotentRequest = false;
            opt.RetryCount = 1;
            opt.RetryDelay = 2;
        });

        var client =
            _services.BuildServiceProvider().GetRequiredService<ITestHttpClient>();

        await client.HttpClient.PostAsync("", null);

        Assert.That(tryCount, Is.EqualTo(1));
    }

    [Test]
    public async Task AddHttpClient_WithTimeOutPolicy_CallSendAsync_Twice()
    {
        // В настройках клиента задаём количество повторных попыток 2
        // Задержка между попытками экспоненциальная с шагом 1 сек.
        // Включаяем UseTimeoutPolicy
        // Задаём Timeout = 1 сек.
        var handler = new Mock<DelegatingHandler>(MockBehavior.Strict);
        var tryCount = 0;

        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).Returns(async () =>
            {
                // Если выполняется первая, ставим задержку в 1300 мсек.
                // И возвращаем HTTP 502.
                // После первой попытки возвращаем HTTP 200 без задержек.
                if (tryCount >= 1)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }

                await Task.Delay(1300);
                return new HttpResponseMessage(HttpStatusCode.BadGateway);
            })
            .Callback(() => tryCount += 1)
            .Verifiable();

        _services.AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>()
            .AddHttpMessageHandler(() => handler.Object);

        _services.Configure<TestHttpClientOptions>(opt =>
        {
            opt.UseReplayPolicy = true;
            opt.RetryCount = 2;
            opt.RetryDelay = 1;
            opt.UseTimeoutPolicy = true;
            opt.TryTimeout = 1;
        });

        var client =
            _services.BuildServiceProvider().GetRequiredService<ITestHttpClient>();

        await client.HttpClient.GetAsync("");

        // С описанными выше условиями суммарное количество попыток должно равняться 2, а не 3.
        // Так как первая (исходный запрос) попытка остановятся по Timeout,
        // а 2 выполниться успешно, 3 не запускается.
        Assert.That(tryCount, Is.EqualTo(2));
    }

    [Test]
    public async Task AddHttpClient_WithReplayPolicy_OnlyIdempotent_WithPost_NoOpHandled()
    {
        var handler = new Mock<DelegatingHandler>(MockBehavior.Strict);
        var tryCount = 0;

        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway))
            .Callback(() => tryCount += 1)
            .Verifiable();

        _services.AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>()
            .AddHttpMessageHandler(() => handler.Object);

        _services.Configure<TestHttpClientOptions>(opt =>
        {
            opt.UseReplayPolicy = true;
            opt.UseReplayPolicyOnlyIdempotentRequest = true;
            opt.RetryCount = 1;
            opt.RetryDelay = 2;
        });

        var client =
            _services.BuildServiceProvider().GetRequiredService<ITestHttpClient>();

        await client.HttpClient.PostAsync("", null);

        Assert.That(tryCount, Is.EqualTo(1));
    }

    [TestCase("GET")]
    [TestCase("HEAD")]
    [TestCase("PUT")]
    [TestCase("DELETE")]
    public async Task AddHttpClient_WithReplayPolicy_OnlyIdempotent_CallSendAsync_Twice(string method)
    {
        var handler = new Mock<DelegatingHandler>(MockBehavior.Strict);
        var tryCount = 0;

        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway))
            .Callback(() => tryCount += 1)
            .Verifiable();

        _services.AddHttpClient<ITestHttpClient, TestHttpClient, TestHttpClientOptions>()
            .AddHttpMessageHandler(() => handler.Object);

        _services.Configure<TestHttpClientOptions>(opt =>
        {
            opt.UseReplayPolicy = true;
            opt.UseReplayPolicyOnlyIdempotentRequest = true;
            opt.RetryCount = 1;
            opt.RetryDelay = 2;
        });

        var client =
            _services.BuildServiceProvider().GetRequiredService<ITestHttpClient>();

        await client.HttpClient.SendAsync(new HttpRequestMessage(new HttpMethod(method), ""));

        Assert.That(tryCount, Is.EqualTo(2));
    }
}