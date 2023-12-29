using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Messaging.Diagnostic;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.Tests.DSL;

namespace ViennaNET.Messaging.Tests.Diagnostic;

[TestFixture(Category = "Unit")]
[TestOf(typeof(MessagingConnectionChecker))]
public class MessagingConnectionCheckerTests
{
    private const string MqSeriesId = "MqSeriesId";
    private const string MqSeriesServer = "MqSeriesServer";

    private static MqSeriesQueueConfiguration CreateConfiguration(bool isHealthCheck = true)
    {
        return new MqSeriesQueueConfiguration
        {
            IsHealthCheck = isHealthCheck,
            Id = MqSeriesId,
            Server = MqSeriesServer
        };
    }

    [Test]
    public async Task Diagnose_CorrectConnection_CorrectResult()
    {
        var checker = Given.MessagingConnectionChecker
            .WithConstructor(
                c => c.WithAdapter(
                    a => a.WithQueueConfiguration(CreateConfiguration()),
                    MqSeriesId))
            .Please();

        var result = await checker.Diagnose();
        var diagnosticInfo = result.First();

        Assert.Multiple(() =>
        {
            Assert.That(diagnosticInfo.Error, Is.EqualTo(string.Empty));
            Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.Ok));
            Assert.That(diagnosticInfo.IsSkipResult, Is.False);
            Assert.That(diagnosticInfo.Name, Is.EqualTo(MqSeriesId));
            Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public async Task Diagnose_ErrorOnConnect_ErrorResult()
    {
        const string errorText = "Error_connect";
        var checker = Given.MessagingConnectionChecker
            .WithConstructor(
                c => c.WithAdapter(
                    a => a.WithQueueConfiguration(CreateConfiguration()).Please(
                        m => m.Setup(x => x.Connect()).Throws(new Exception(errorText))),
                    MqSeriesId))
            .Please();

        var result = await checker.Diagnose();

        var diagnosticInfo = result.First();

        Assert.Multiple(() =>
        {
            Assert.That(diagnosticInfo.Error, Does.Contain(errorText));
            Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.QueueError));
            Assert.That(diagnosticInfo.IsSkipResult, Is.False);
            Assert.That(diagnosticInfo.Name, Is.EqualTo(MqSeriesId));
            Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public async Task Diagnose_ErrorOnCreateAll_ErrorResult()
    {
        const string errorText = "Error_connect";
        var messageAdapterConstructor = new Mock<IMessageAdapterConstructor>();
        messageAdapterConstructor.Setup(x => x.CreateAll()).Throws(new Exception(errorText));
        
        var checker = Given.MessagingConnectionChecker.WithConstructor(messageAdapterConstructor.Object).Please();
        var result = await checker.Diagnose();
        var diagnosticInfo = result.First();

        Assert.Multiple(() =>
        {
            Assert.That(diagnosticInfo.Error, Does.Contain(errorText));
            Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.ConfigError));
            Assert.That(diagnosticInfo.IsSkipResult, Is.False);
            Assert.That(diagnosticInfo.Name, Is.EqualTo("constructor"));
            Assert.That(diagnosticInfo.Url, Is.Null);
            Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public async Task Diagnose_ErrorOnDisconnect_ErrorResult()
    {
        const string errorText = "Error_connect";

        var checker = Given.MessagingConnectionChecker
            .WithConstructor(
                c => c.WithAdapter(
                    a => a.WithQueueConfiguration(CreateConfiguration()).Please(
                        m => m.Setup(x => x.Disconnect()).Throws(new Exception(errorText))),
                    MqSeriesId))
            .Please();

        var result = await checker.Diagnose();
        var diagnosticInfo = result.First();

        Assert.Multiple(() =>
        {
            Assert.That(diagnosticInfo.Error, Does.Contain(errorText));
            Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.QueueError));
            Assert.That(diagnosticInfo.IsSkipResult, Is.False);
            Assert.That(diagnosticInfo.Name, Is.EqualTo(MqSeriesId));
            Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public async Task Diagnose_NoAdaptersReturned_EmptyResult()
    {
        var checker = Given.MessagingConnectionChecker.WithConstructor(Given.MessageAdapterConstructor).Please();

        var result = await checker.Diagnose();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Diagnose_NoConstructors_EmptyResult()
    {
        var checker = Given.MessagingConnectionChecker.Please();
        var result = await checker.Diagnose();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Diagnose_NoHealthCheckConnections_EmptyResult()
    {
        var checker = Given.MessagingConnectionChecker
            .WithConstructor(
                c => c.WithAdapter(
                    a => a.WithQueueConfiguration(CreateConfiguration(false)),
                    MqSeriesId))
            .Please();

        var result = await checker.Diagnose();

        Assert.That(result, Is.Empty);
    }
}