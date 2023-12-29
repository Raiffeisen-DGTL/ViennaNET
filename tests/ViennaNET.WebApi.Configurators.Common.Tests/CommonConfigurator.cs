using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Configurators.Common.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(Common.CommonConfigurator))]
  public class CommonConfigurator
  {
    [SetUp]
    public void SetUp()
    {
      _companyHostBuilderMock = new Mock<ICompanyHostBuilder>();
      _appBuilderMock = new Mock<IApplicationBuilder>();
      _configMock = new Mock<IConfiguration>();
      _envMock = new Mock<IHostEnvironment>();

      var configSectionMock = new Mock<IConfigurationSection>();

      _companyHostBuilderMock
        .Setup(builder => builder.ConfigureApp(
          It.IsAny<Action<IApplicationBuilder, IConfiguration, IHostEnvironment, object>>(), It.IsAny<bool>()))
        .Callback<Action<IApplicationBuilder, IConfiguration, IHostEnvironment, object>, bool>(
          (appConfigurationAction, initBeforeContainer)
            => appConfigurationAction(_appBuilderMock.Object, _configMock.Object, _envMock.Object, null))
        .Returns(_companyHostBuilderMock.Object);
      
      _configMock
        .Setup(configuration => configuration.GetSection(It.IsAny<string>()))
        .Returns(configSectionMock.Object);
    }
    
    private Mock<ICompanyHostBuilder> _companyHostBuilderMock;
    private Mock<IApplicationBuilder> _appBuilderMock;
    private Mock<IConfiguration> _configMock;
    private Mock<IHostEnvironment> _envMock;

    [Test]
    public void ConfigureLoggerMiddleware()
    {
      _companyHostBuilderMock.Object.UseCommonModules();
      _appBuilderMock.Verify(
        builder => builder.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
    }
  }
}