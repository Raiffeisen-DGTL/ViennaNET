using System.Linq;
using System.Net;
using System.Net.Http;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Threading;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Handlers;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.CallContext;

namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Tests.HttpClientConfiguration
{
  [TestFixture, Category("Unit"), TestOf(typeof(BaseCompanyRequestHeadersHandler))]
  public class CompanyRequestHeadersHandlerTests
  {
    private class FakeHttpClientHandler : HttpClientHandler
    {
      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
      }
    }

    [Test]
    public async Task SetHeaderParameters_HasValuesInContext_SetHeaders()
    {
      // arrange
      var fakeContext = new Mock<ICallContext>();
      fakeContext.Setup(x => x.UserId)
                 .Returns("hamster");
      fakeContext.Setup(x => x.UserDomain)
                 .Returns("Domain");
      fakeContext.Setup(x => x.RequestCallerIp)
                 .Returns("0.0.0.0");
      fakeContext.Setup(x => x.AuthorizeInfo)
                 .Returns("token");
      fakeContext.Setup(x => x.RequestId)
                 .Returns("123");

      var fakeContextFactory = new Mock<ICallContextFactory>();
      fakeContextFactory.Setup(x => x.Create())
                        .Returns(fakeContext.Object);

      var handler = new BaseCompanyRequestHeadersHandler(fakeContextFactory.Object)
      {
        InnerHandler = new FakeHttpClientHandler()
      };
      var client = new System.Net.Http.HttpClient(handler);

      // act
      var request = new HttpRequestMessage(HttpMethod.Get, "http://0.0.0.0:80");
      await client.SendAsync(request);

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(request.Headers.GetValues(CompanyHttpHeaders.UserId)
                           .First(), Is.EqualTo(fakeContext.Object.UserId));
        Assert.That(request.Headers.GetValues(CompanyHttpHeaders.UserDomain)
                           .First(), Is.EqualTo(fakeContext.Object.UserDomain));
        Assert.That(request.Headers.GetValues(CompanyHttpHeaders.RequestId)
                           .First(), Is.EqualTo(fakeContext.Object.RequestId));
        Assert.That(request.Headers.GetValues(CompanyHttpHeaders.RequestHeaderCallerIp)
                           .First(), Is.EqualTo(fakeContext.Object.RequestCallerIp));
      });
    }
  }
}
