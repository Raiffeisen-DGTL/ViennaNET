using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Threading;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Handlers;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Tests.HttpClientConfiguration
{
  [TestFixture, Category("Unit"), TestOf(typeof(BaseCompanyRequestHeadersHandler))]
  public class CompanyRequestHeadersHandlerTests
  {
    private class FakeHttpClientHandler : HttpClientHandler
    {
      public HttpRequestMessage RequestMessage;

      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
        RequestMessage = request;
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
      }
    }

    [Test]
    public async Task SetHeaderParameters_DontHaveIncomingRequest_SetDefaultHeaders()
    {
      // arrange
      var fakeRequest = new Mock<HttpRequest>();
      var fakeContext = new Mock<HttpContext>();

      fakeRequest.Setup(x => x.Headers)
                 .Returns(new HeaderDictionary());
      fakeRequest.Setup(x => x.HttpContext)
                 .Returns(fakeContext.Object);
      fakeContext.Setup(x => x.Request)
                 .Returns(fakeRequest.Object);

      var fakeContextAccessor = new Mock<IHttpContextAccessor>();
      fakeContextAccessor.Setup(x => x.HttpContext)
                         .Returns(fakeContext.Object);

      var handler = new BaseCompanyRequestHeadersHandler(fakeContextAccessor.Object);
      handler.InnerHandler = new FakeHttpClientHandler();
      var client = new System.Net.Http.HttpClient(handler);

      // act
      var request = new HttpRequestMessage(HttpMethod.Get, "http://0.0.0.0:80");
      await client.SendAsync(request);

      // assert
      Assert.That(request.Headers.GetValues(CompanyHttpHeaders.RequestId)
                         .First(), Is.Not.Empty);
      Assert.That(request.Headers.GetValues(CompanyHttpHeaders.UserId)
                         .First(), Is.EqualTo(Environment.UserName));
      Assert.That(request.Headers.GetValues(CompanyHttpHeaders.UserDomain)
                         .First(), Is.EqualTo(Environment.UserDomainName));
      request.Headers.TryGetValues(CompanyHttpHeaders.RequestHeaderCallerIp, out var values);
      Assert.That(values, Is.Null);
    }

    [TestCase(CompanyHttpHeaders.UserId, "some user id")]
    [TestCase(CompanyHttpHeaders.RequestHeaderCallerIp, "some caller ip")]
    [TestCase(CompanyHttpHeaders.RequestId, "some request id")]
    [TestCase(CompanyHttpHeaders.UserDomain, "some user domain")]
    public async Task SetHeaderParameters_HasIncomingRequestWithHeader_SetHeaderFromIncoming(string headerKey, string headerValue)
    {
      // arrange
      var fakeRequest = new Mock<HttpRequest>();
      var fakeContext = new Mock<HttpContext>();
      fakeRequest.Setup(x => x.Headers)
                 .Returns(new HeaderDictionary() { new KeyValuePair<string, StringValues>(headerKey, new StringValues(headerValue)) });

      fakeRequest.Setup(x => x.HttpContext)
                 .Returns(fakeContext.Object);
      fakeContext.Setup(x => x.Request)
                 .Returns(fakeRequest.Object);

      var fakeContextAccessor = new Mock<IHttpContextAccessor>();
      fakeContextAccessor.Setup(x => x.HttpContext)
                         .Returns(fakeContext.Object);

      var handler = new BaseCompanyRequestHeadersHandler(fakeContextAccessor.Object);
      handler.InnerHandler = new FakeHttpClientHandler();
      var client = new System.Net.Http.HttpClient(handler);

      // act
      var request = new HttpRequestMessage(HttpMethod.Get, "http://0.0.0.0:80");
      await client.SendAsync(request);

      // assert
      Assert.That(request.Headers.GetValues(headerKey)
                         .First(), Is.EqualTo(headerValue));
    }

    [Test]
    public async Task GetCallerIp_HasRequestWithoutCallerIpHeader_SetCallerIpFromConnection()
    {
      // arrange
      var fakeRequest = new Mock<HttpRequest>();
      fakeRequest.Setup(x => x.Headers)
                 .Returns(new HeaderDictionary());

      var fakeContext = new Mock<HttpContext>();
      fakeRequest.Setup(x => x.HttpContext)
                 .Returns(fakeContext.Object);
      fakeContext.Setup(x => x.Request)
                 .Returns(fakeRequest.Object);
      var fakeConnectionInfo = new Mock<ConnectionInfo>();
      fakeConnectionInfo.Setup(x => x.RemoteIpAddress)
                        .Returns(new IPAddress(123));

      fakeContext.Setup(x => x.Connection)
                 .Returns(fakeConnectionInfo.Object);

      var fakeContextAccessor = new Mock<IHttpContextAccessor>();
      fakeContextAccessor.Setup(x => x.HttpContext)
                         .Returns(fakeContext.Object);

      var handler = new BaseCompanyRequestHeadersHandler(fakeContextAccessor.Object);
      handler.InnerHandler = new FakeHttpClientHandler();
      var client = new System.Net.Http.HttpClient(handler);

      // act
      var request = new HttpRequestMessage(HttpMethod.Get, "http://0.0.0.0:80");
      await client.SendAsync(request);

      // assert
      Assert.That(request.Headers.GetValues(CompanyHttpHeaders.RequestHeaderCallerIp)
                         .First(), Is.EqualTo(new IPAddress(123).ToString()));
    }
  }
}
