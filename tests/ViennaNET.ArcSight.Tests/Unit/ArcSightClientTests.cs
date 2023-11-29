using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Polly;
using ViennaNET.ArcSight.Configuration;
using ViennaNET.ArcSight.Exceptions;

namespace ViennaNET.ArcSight.Tests.Unit
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(ArcSightClient))]
  public class ArcSightClientTests
  {
    [SetUp]
    public void SetUp()
    {
      _syncPolicyMock = new Mock<ISyncPolicy>();
      _syncPolicyMock.Setup(x => x.Execute(It.IsAny<Action>()))
        .Callback<Action>(action => action());
      _errorHandlingFactoryMock = new Mock<IErrorHandlingPoliciesFactory>();
      _errorHandlingFactoryMock.Setup(x => x.CreateStdCommunicationPolicy())
        .Returns(_syncPolicyMock.Object);

      _cefSenderMock = new Mock<ICefSender>();
      _cefSenderFactoryMock = new Mock<ICefSenderFactory>();
      _cefSenderFactoryMock.Setup(x => x.CreateSender(It.IsAny<ArcSightSection>()))
        .Returns(_cefSenderMock.Object);
    }

    private Mock<ISyncPolicy> _syncPolicyMock;
    private Mock<IErrorHandlingPoliciesFactory> _errorHandlingFactoryMock;
    private Mock<ICefSenderFactory> _cefSenderFactoryMock;
    private Mock<ICefSender> _cefSenderMock;

    private IArcSightClient CreateClient(string configRoot)
    {
      var configurationRoot = new ConfigurationBuilder().AddJsonFile(configRoot, true)
        .Build();
      return new ArcSightClient(configurationRoot, _errorHandlingFactoryMock.Object, _cefSenderFactoryMock.Object,
        new NullLogger<ArcSightClient>());
    }

    [Test]
    public void Send_CorrectSettingsAndMessage_MessageCorrectlySent()
    {
      var client = CreateClient("TestConfiguration/arcSight.json");

      client.Send(new CefMessage(DateTime.Today, "host", "Security", "threatmanager", "1.0", 100,
        "worm successfullystopped",
        CefSeverity.Emergency));

      _cefSenderFactoryMock.Verify(x => x.CreateSender(It.IsAny<ArcSightSection>()), Times.Once);
      _errorHandlingFactoryMock.Verify(x => x.CreateStdCommunicationPolicy(), Times.Once);
      _syncPolicyMock.Verify(x => x.Execute(It.IsAny<Action>()), Times.Once);
      _cefSenderMock.Verify(x => x.Send(It.IsAny<CefMessage>(), It.IsAny<CefMessageSerializer>()), Times.Once);
    }

    [Test]
    public void Ctor_IncorrectSyslogVersion_MessageDidntSend()
    {
      Assert.Throws<ArcSightConfigurationException>(
        () => CreateClient("TestConfiguration/arcSightIncorrectSyslogVersion.json"),
        "The syslog version rfc has not supported");
    }
  }
}