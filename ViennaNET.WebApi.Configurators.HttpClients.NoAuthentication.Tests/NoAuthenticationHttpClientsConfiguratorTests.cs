using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using ViennaNET.CallContext;

namespace ViennaNET.WebApi.Configurators.HttpClients.NoAuthentication.Tests
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(NoAuthenticationHttpClientsConfigurator))]
  public class NoAuthenticationHttpClientsConfiguratorTests
  {
    private static  ServiceCollection GetServiceCollection()
    {
      var serviceCollection = new ServiceCollection();

      var callContextFactory = new Mock<ICallContextFactory>();
      callContextFactory
        .Setup(c => c.Create())
        .Returns(new EmptyCallContext());

      serviceCollection.AddSingleton(callContextFactory.Object);

      return serviceCollection;
    }

    [Test]
    public void RegisterHttpClients_ShouldRegisterHttpClient()
    {
      // Arrange
      var serviceCollection = GetServiceCollection();

      var configurationSection =
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
          {"webApiEndpoints:0:Name", "noAuthHttpClient"},
          {"webApiEndpoints:0:Url", "http://localhost"},
          {"webApiEndpoints:0:Timeout", "1"},
          {"webApiEndpoints:0:AuthType", "noauth"}
        })
          .Build();

      // Act
      serviceCollection.RegisterHttpClients(configurationSection);

      // Assert
      var buildServiceProvider = serviceCollection.BuildServiceProvider();
      var httpClientFactory = (IHttpClientFactory)buildServiceProvider.GetService(typeof(IHttpClientFactory));
      var httpClient = httpClientFactory.CreateClient("noAuthHttpClient");

      Assert.That(httpClient.DefaultRequestHeaders.Authorization, Is.Null);
    }

    [Test]
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
      Assert.That(httpClient.Timeout.TotalSeconds, Is.EqualTo(defaultTimeoutValue));
    }

    [TestCase("ntlm", TestName = "RegisterHttpClients_RegistringNtlmAuthType_ShouldNotRegisterHttpClient")]
    [TestCase("jwt", TestName = "RegisterHttpClients_RegistringJwtAuthType_ShouldNotRegisterHttpClient")]
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

      Assert.That(httpClientFactory, Is.Null);
    }
  }
}