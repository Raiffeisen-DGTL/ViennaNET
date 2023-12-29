using System.Net;
using System.Text;
using NUnit.Framework;

namespace ViennaNET.WebApi.Configurators.HttpClients.Basic.Tests;

[TestFixture]
[Category("Unit")]
[TestOf(typeof(BasicHttpClientAuthorizationRequestHandler))]
public class BasicHttpClientAuthorizationRequestHandlerTests
{
    private class FakeHttpClientHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    [Test]
    public async Task Handle_BasicAuth_ShouldSetHeaders()
    {
        const string userName = "testUser";
        const string password = "testPassword";

        var handler = new BasicHttpClientAuthorizationRequestHandler(userName, password)
        {
            InnerHandler = new FakeHttpClientHandler()
        };

        var client = new System.Net.Http.HttpClient(handler);

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
        await client.SendAsync(request);

        Assert.Multiple(() =>
        {
            var expectedCreds = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));

            Assert.That(request.Headers.Authorization?.Scheme, Is.EqualTo("Basic"));
            Assert.That(expectedCreds, Is.EqualTo(request.Headers.Authorization?.Parameter));
        });
    }
}