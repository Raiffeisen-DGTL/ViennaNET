using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Polly;
using ViennaNET.ArcSight.Configuration;
using ViennaNET.ArcSight.Exceptions;

namespace ViennaNET.ArcSight.Tests.Unit;

[TestFixture]
[Category("Unit")]
[TestOf(typeof(ArcSightClient))]
public class ArcSightClientTests
{
    [SetUp]
    public void SetUp()
    {
        var builder = new ConfigurationBuilder();

        builder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("arcSight:serverHost", "localhost"),
            new("arcSight:serverPort", "60"),
            new("arcSight:syslogVersion", "rfc3164"),
            new("arcSight:protocol", "tcp")
        });

        _configuration = builder.Build();
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

    private IConfiguration _configuration = null!;
    private Mock<ISyncPolicy> _syncPolicyMock = null!;
    private Mock<IErrorHandlingPoliciesFactory> _errorHandlingFactoryMock = null!;
    private Mock<ICefSenderFactory> _cefSenderFactoryMock = null!;
    private Mock<ICefSender> _cefSenderMock = null!;

    [Test]
    public void Send_CorrectSettingsAndMessage_MessageCorrectlySent()
    {
        var policyFactory = _errorHandlingFactoryMock.Object;
        var senderFactory = _cefSenderFactoryMock.Object;
        var logger = new NullLogger<ArcSightClient>();
        var client = new ArcSightClient(_configuration, policyFactory, senderFactory, logger);

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
        var builder = new ConfigurationBuilder();

        builder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("arcSight:serverHost", "localhost"),
            new("arcSight:serverPort", "60"),
            new("arcSight:syslogVersion", "rfc"),
            new("arcSight:protocol", "tcp")
        });
        var config = builder.Build();
        var policyFactory = _errorHandlingFactoryMock.Object;
        var senderFactory = _cefSenderFactoryMock.Object;
        var logger = new NullLogger<ArcSightClient>();

        Assert.That(() => new ArcSightClient(config, policyFactory, senderFactory, logger),
            Throws.InstanceOf<ArcSightConfigurationException>().And
                .Message.EqualTo("The syslog version rfc has not supported"));
    }
}