using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Company.WebApi.Core.DefaultConfiguration.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Company.WebApi.Core.DefaultConfiguration.HttpClients.Handlers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using System.Threading;

namespace Company.WebApi.Core.DefaultConfiguration.Tests.HttpClientConfiguration
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
      Assert.That(request.Headers.GetValues(CompanyHeaders.RequestId)
                         .First(), Is.Not.Empty);
      Assert.That(request.Headers.GetValues(CompanyHeaders.UserId)
                         .First(), Is.EqualTo(Environment.UserName));
      Assert.That(request.Headers.GetValues(CompanyHeaders.UserDomain)
                         .First(), Is.EqualTo(Environment.UserDomainName));
      request.Headers.TryGetValues(CompanyHeaders.RequestHeaderCallerIp, out var values);
      Assert.That(values, Is.Null);
    }

    [TestCase(CompanyHeaders.UserId, "some user id")]
    [TestCase(CompanyHeaders.RequestHeaderCallerIp, "some caller ip")]
    [TestCase(CompanyHeaders.RequestId, "some request id")]
    [TestCase(CompanyHeaders.UserDomain, "some user domain")]
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
      fakeContext.Setup(x => x.Connection)
                 .Returns(new DefaultConnectionInfo(new FeatureCollection()) { RemoteIpAddress = new IPAddress(123) });

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
      Assert.That(request.Headers.GetValues(CompanyHeaders.RequestHeaderCallerIp)
                         .First(), Is.EqualTo(new IPAddress(123).ToString()));
    }
  }
}
