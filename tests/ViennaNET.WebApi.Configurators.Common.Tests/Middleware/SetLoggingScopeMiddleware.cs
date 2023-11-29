using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ViennaNET.WebApi.Configurators.Common.Middleware.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(Middleware.SetLoggingScopeMiddleware))]
  public class SetLoggingScopeMiddleware
  {
    [SetUp]
    public void SetUp()
    {
      _loggerMock = new Mock<ILogger<SetLoggingScopeMiddleware>>();
      _loggerProviderMock = new Mock<ILoggerProvider>();
      _loggerProviderMock.Setup(provider => provider.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);

      _hostBuilder = new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
          webBuilder
            .UseTestServer()
            .ConfigureLogging(builder => builder.AddProvider(_loggerProviderMock.Object))
            .Configure(app =>
            {
              var logger = (ILogger<SetLoggingScopeMiddleware>)
                app.ApplicationServices.GetService(typeof(ILogger<SetLoggingScopeMiddleware>));

              app.Use((context, func) =>
                {
                  var activity = new Activity("Test").Start();
                  logger.LogInformation("test");
                  return func();
                })
                .UseMiddleware<Middleware.SetLoggingScopeMiddleware>();
            });
        });
    }

    private IHostBuilder _hostBuilder;
    private Mock<ILogger<SetLoggingScopeMiddleware>> _loggerMock;
    private Mock<ILoggerProvider> _loggerProviderMock;

    [TestCase("", "", TestName = "test invoke method with a given empty X-Request-Id and X-User-Id")]
    [TestCase("ANY_DOMAIN\\any_user", "1234567890",
      TestName = "test invoke method with the given X-Request-Id and X-User-Id contains the domain")]
    [TestCase("any_user", "1234567890",
      TestName = "test invoke method with a given X-Request-Id and a not contained domain X-User-Id")]
    public async Task Invoke(string username, string requestId)
    {
      const string issuer = "LOCAL AUTHORITY";

      var claimName = new Claim(ClaimTypes.Name, username, ClaimValueTypes.String, issuer, issuer);
      var identity = new ClaimsIdentity(string.Empty, ClaimTypes.Name, ClaimTypes.Role);
      var principal = new ClaimsPrincipal();
      using var host = await _hostBuilder.StartAsync();
      var server = host.GetTestServer();

      identity.AddClaim(claimName);
      principal.AddIdentity(identity);

      Assert.Multiple(() =>
      {
        Assert.DoesNotThrowAsync(async () => await server.SendAsync(c =>
        {
          c.User = principal;
          c.Request.Headers.Add("X-Request-Id", requestId);
          c.Request.Headers.Add("X-User-Id", username);
        }));
        _loggerMock.Verify(logger => logger.BeginScope(It.IsAny<string>()), Times.Once());
      });
    }
  }
}