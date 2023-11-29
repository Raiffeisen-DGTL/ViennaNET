using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using ViennaNET.Diagnostic.Data;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Diagnostic;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Tests.Unit.DSL;

namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Tests.Unit.Diagnostic
{
  [TestFixture(Category = "Unit")]
  [TestOf(typeof(HttpEndpointsChecker))]
  public class HttpEndpointsCheckerTests
  {
    [Test]
    public async Task Diagnose_HasHealthyEndpoint_ReturnsOkResult()
    {
      // arrange
      var config = Given.Configuration
        .WithWebapiEndpoint(new WebapiEndpoint { Name = "endpoint1", Url = "http://localhost" })
        .Build();
      var httpClientFactory = Given.HttpClientFactory.WithResponse("/diagnostic/ping", HttpStatusCode.OK).Build();

      // act
      var checker = new HttpEndpointsChecker(config, httpClientFactory, new NullLogger<HttpEndpointsChecker>());
      var result = await checker.Diagnose();

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Status, Is.EqualTo(DiagnosticStatus.Ok));
      });
    }

    [Test]
    public async Task Diagnose_HasUnhealthyEndpoint_ReturnsPingErrorResult()
    {
      // arrange
      var config = Given.Configuration
        .WithWebapiEndpoint(new WebapiEndpoint { Name = "endpoint1", Url = "http://localhost" })
        .Build();
      var httpClientFactory =
        Given.HttpClientFactory.WithResponse("/diagnostic/ping", HttpStatusCode.InternalServerError).Build();

      // act
      var checker = new HttpEndpointsChecker(config, httpClientFactory, new NullLogger<HttpEndpointsChecker>());
      var result = await checker.Diagnose();

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Status, Is.EqualTo(DiagnosticStatus.PingError));
      });
    }

    [Test]
    public async Task Diagnose_HasEndpointWithDisabledHealthCheck_DiagnosticNotCalled()
    {
      // arrange
      var config = Given.Configuration
        .WithWebapiEndpoint(new WebapiEndpoint { Name = "endpoint1", Url = "http://localhost", IsHealthCheck = false })
        .Build();
      var httpClientFactory =
        Given.HttpClientFactory.WithResponse("/diagnostic/ping", HttpStatusCode.InternalServerError).BuildMock();

      // act
      var checker = new HttpEndpointsChecker(config, httpClientFactory.Object, new NullLogger<HttpEndpointsChecker>());
      var result = await checker.Diagnose();

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(result.Count(), Is.EqualTo(0));
        httpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()), Times.Never);
      });
    }
  }
}