using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using ViennaNET.CallContext;

namespace ViennaNET.WebApi.Configurators.HttpClients.Basic.Tests
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(BasicHttpClientsConfigurator))]
  public class BasicHttpClientsConfiguratorTests
  {
    private static ServiceCollection GetServiceCollection()
    {
      var serviceCollection = new ServiceCollection();

      var callContextFactory = new Mock<ICallContextFactory>();
      callContextFactory
        .Setup(c => c.Create())
        .Returns(new EmptyCallContext());

      serviceCollection.AddSingleton(callContextFactory.Object);

      return serviceCollection;
    }

    [TestCase(TestName = "Try create client with basic auth")]
    public void RegisterHttpClients_ShouldRegisterHttpClient()
    {
      // Arrange
      const string userName = "testUser";
      const string password = "testPassword";

      var serviceCollection = GetServiceCollection();

      var configurationSection =
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
          {
            {"webApiEndpoints:0:Name", "basicHttpClient"},
            {"webApiEndpoints:0:Url", "http://localhost"},
            {"webApiEndpoints:0:Timeout", "123"},
            {"webApiEndpoints:0:UserName", userName},
            {"webApiEndpoints:0:Password", password},
            {"webApiEndpoints:0:AuthType", "basic"}
          })
          .Build();

      // Act
      serviceCollection.RegisterHttpClients(configurationSection);

      // Assert
      var buildServiceProvider = serviceCollection.BuildServiceProvider();
      var httpClientFactory = (IHttpClientFactory) buildServiceProvider.GetService(typeof(IHttpClientFactory));
      var httpClient = httpClientFactory.CreateClient("basicHttpClient");

      Assert.IsNotNull(httpClient);
    }

    [TestCase(TestName = "Try create client with default settings")]
    public void RegisterHttpClients_WhenNoEndpoints_ShouldAddDefaultHttpClient()
    {
      // Arrange
      var serviceCollection = GetServiceCollection();

      var configurationSection =
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>())
          .Build();

      // Act
      serviceCollection.RegisterHttpClients(configurationSection);

      // Assert
      var buildServiceProvider = serviceCollection.BuildServiceProvider();
      var httpClientFactory = (IHttpClientFactory)buildServiceProvider.GetService(typeof(IHttpClientFactory));
      var httpClient = httpClientFactory.CreateClient("someHttpClient");

      const int defaultTimeoutValue = 100;
      Assert.AreEqual(defaultTimeoutValue, httpClient.Timeout.TotalSeconds);
    }

    [TestCase("ntlm", TestName = "Try register client(ntlm)")]
    [TestCase("jwt", TestName = "Try register client(jwt)")]
    [TestCase("noauth", TestName = "Try register client(noauth)")]
    public void RegisterHttpClients_ShouldNotRegisterHttpClient(string authType)
    {
      // Arrange
      var serviceCollection = GetServiceCollection();

      var configurationSection =
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
          {
            {"webApiEndpoints:0:Name", "someHttpClient"},
            {"webApiEndpoints:0:Url", "http://localhost"},
            {"webApiEndpoints:0:Timeout", "1"},
            {"webApiEndpoints:0:AuthType", authType}
          })
          .Build();

      // Act
      serviceCollection.RegisterHttpClients(configurationSection);

      // Assert
      var buildServiceProvider = serviceCollection.BuildServiceProvider();
      var httpClientFactory = (IHttpClientFactory)buildServiceProvider.GetService(typeof(IHttpClientFactory));

      Assert.IsNull(httpClientFactory);
    }
  }
}