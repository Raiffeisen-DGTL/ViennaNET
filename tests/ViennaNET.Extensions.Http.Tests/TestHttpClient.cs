namespace ViennaNET.Extensions.Http.Tests;

public interface ITestHttpClient
{
    public HttpClient HttpClient { get; }
}

internal sealed class TestHttpClient : ITestHttpClient
{
    public TestHttpClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public HttpClient HttpClient { get; }
}

public class TestHttpClientOptions : ClientOptionsBase
{
    public const string SectionName = DefaultSectionRootName + "TestApi";
}