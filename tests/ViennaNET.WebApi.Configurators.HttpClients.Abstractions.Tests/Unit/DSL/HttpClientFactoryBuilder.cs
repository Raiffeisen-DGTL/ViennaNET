using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Tests.Unit.DSL
{
  internal class HttpClientFactoryBuilder
  {
    private readonly Dictionary<string, ResponseData> _responses = new();

    public HttpClientFactoryBuilder WithResponse(string url, HttpStatusCode code)
    {
      _responses.Add(url, new ResponseData(code));
      return this;
    }

    public IHttpClientFactory Build()
    {
      return CreateFactoryMock(_responses).Object;
    }

    public Mock<IHttpClientFactory> BuildMock()
    {
      return CreateFactoryMock(_responses);
    }

    private static Mock<IHttpClientFactory> CreateFactoryMock(Dictionary<string, ResponseData> responses)
    {
      var fakeHttpClient = new System.Net.Http.HttpClient(new FakeResponseHandler(responses));
      fakeHttpClient.BaseAddress = new Uri("http://localhost");

      var fakeFactory = new Mock<IHttpClientFactory>();
      fakeFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
        .Returns(fakeHttpClient);

      return fakeFactory;
    }

    internal class FakeResponseHandler : HttpMessageHandler
    {
      private readonly Dictionary<string, ResponseData> _responses;

      public FakeResponseHandler(Dictionary<string, ResponseData> responses)
      {
        _responses = responses;
      }

      protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
      {
        var response = _responses[request.RequestUri.LocalPath];

        return Task.FromResult(new HttpResponseMessage(response.Code));
      }
    }

    internal class ResponseData
    {
      public HttpStatusCode Code;

      public ResponseData(HttpStatusCode code)
      {
        Code = code;
      }
    }
  }
}