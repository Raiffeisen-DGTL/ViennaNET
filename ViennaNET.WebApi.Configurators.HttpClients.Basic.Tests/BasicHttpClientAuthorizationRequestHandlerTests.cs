using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.WebApi.Configurators.HttpClients.Basic.Tests
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(BasicHttpClientAuthorizationRequestHandler))]
  public class BasicHttpClientAuthorizationRequestHandlerTests
  {
    private class FakeHttpClientHandler : HttpClientHandler
    {
      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
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

        Assert.AreEqual("Basic", request.Headers.Authorization?.Scheme);
        Assert.AreEqual(expectedCreds, request.Headers.Authorization?.Parameter);
      });
    }
  }
}