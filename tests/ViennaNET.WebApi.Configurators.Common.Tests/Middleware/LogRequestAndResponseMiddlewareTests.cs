using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using ViennaNET.Logging;
using ViennaNET.Logging.Contracts;

namespace ViennaNET.WebApi.Configurators.Common.Middleware.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(LogRequestAndResponseMiddleware))]
  public class LogRequestAndResponseMiddlewareTests
  {
    [SetUp]
    public void SetUp()
    {
      _loggerMock = new Mock<ILog>();
      Logger.Configure(_loggerMock.Object);

      _hostBuilder = new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
          webBuilder
            .UseTestServer()
            .Configure(app =>
            {
              app
                .UseMiddleware<LogRequestAndResponseMiddleware>();
            });
        });
    }

    private IHostBuilder _hostBuilder;
    private Mock<ILog> _loggerMock;

    [TestCase("test", 4, "test", TestName = "Logs request body for request with non-empty body and Content-Length")]
    [TestCase("test", null, "test",
      TestName = "Logs request body for request with non-empty body and no Content-Length")]
    [TestCase("", null, "application/json", TestName = "Logs request Content-Type for request with empty body")]
    public async Task Invoke(string body, int? contentLength, string expectedLogLine)
    {
      const string contentType = "application/json";

      using var host = await _hostBuilder.StartAsync();
      var server = host.GetTestServer();

      Assert.Multiple(() =>
      {
        Assert.DoesNotThrowAsync(async () => await server.SendAsync(c =>
        {
          c.Request.Headers.Add("Content-Type", contentType);
          if (contentLength != null)
          {
            c.Request.Headers.Add("Content-Length", $"{contentLength}");
          }

          c.Request.Body = new MemoryStream();
          var bodyWriter = new StreamWriter(c.Request.Body);
          bodyWriter.Write(body);
          bodyWriter.Flush();
          c.Request.Body.Position = 0;
        }));

        Expression<Func<string, bool>> logMessageMatch =
          logMessage => logMessage.Split(new[] { '\n' }).Last() == expectedLogLine;

        _loggerMock.Verify(x => x.Log(It.IsAny<string>(), LogLevel.Debug, It.Is(logMessageMatch)));
      });
    }
  }
}