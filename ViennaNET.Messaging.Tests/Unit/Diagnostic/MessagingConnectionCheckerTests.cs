using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Messaging.Diagnostic;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.MQSeriesQueue;

namespace ViennaNET.Messaging.Tests.Unit.Diagnostic
{
  [TestFixture(Category = "Unit"), TestOf(typeof(MessagingConnectionChecker))]
  public class MessagingConnectionCheckerTests
  {
    private const string MqSeriesId = "MqSeriesId";
    private const string MqSeriesServer = "MqSeriesServer";

    [Test]
    public async Task Diagnose_CorrectConnection_CorrectResult()
    {
      var messageAdapter = new Mock<IMessageAdapter>();
      var messageAdapterConstructor = new Mock<IMessageAdapterConstructor>();
      messageAdapter.Setup(x => x.Configuration)
                    .Returns(CreateConnection());
      messageAdapter.Setup(x => x.Connect());
      messageAdapter.Setup(x => x.Disconnect());
      
      messageAdapterConstructor.Setup(x => x.CreateAll(It.IsAny<bool>()))
                                .Returns(new[] { messageAdapter.Object });

      var checker = new MessagingConnectionChecker(new[] { messageAdapterConstructor.Object });

      var result = await checker.Diagnose();

      var diagnosticInfo = result.First();

      Assert.Multiple(() =>
      {
        Assert.That(diagnosticInfo.Error, Is.EqualTo(string.Empty));
        Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.Ok));
        Assert.That(diagnosticInfo.IsSkipResult, Is.False);
        Assert.That(diagnosticInfo.Name, Is.EqualTo(MqSeriesId));
        Assert.That(diagnosticInfo.Url, Is.EqualTo(MqSeriesServer));
        Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      });
    }

    [Test]
    public async Task Diagnose_NoHealthCheckConnections_EmptyResult()
    {
      var messageAdapter = new Mock<IMessageAdapter>();
      var messageAdapterConstructor = new Mock<IMessageAdapterConstructor>();
      messageAdapter.Setup(x => x.Configuration)
                    .Returns(CreateConnection(false));
      messageAdapter.Setup(x => x.Connect());
      messageAdapter.Setup(x => x.Disconnect());

      messageAdapterConstructor.Setup(x => x.CreateAll(It.IsAny<bool>()))
                                .Returns(new[] { messageAdapter.Object });

      var checker = new MessagingConnectionChecker(new[] { messageAdapterConstructor.Object });

      var result = await checker.Diagnose();

      Assert.That(result, Is.Empty);
    }

    private static MqSeriesQueueConfiguration CreateConnection(bool isHealthCheck = true)
    {
      return new MqSeriesQueueConfiguration { IsHealthCheck = isHealthCheck, Id = MqSeriesId, Server = MqSeriesServer };
    }

    [Test]
    public async Task Diagnose_NoAdaptersReturned_EmptyResult()
    {
      var messageAdapterConstructor = new Mock<IMessageAdapterConstructor>();

      messageAdapterConstructor.Setup(x => x.CreateAll(It.IsAny<bool>()))
                               .Returns(new IMessageAdapter[0]);

      var checker = new MessagingConnectionChecker(new[] { messageAdapterConstructor.Object });

      var result = await checker.Diagnose();

      Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Diagnose_NoConstructors_EmptyResult()
    {
      var checker = new MessagingConnectionChecker(new IMessageAdapterConstructor[0]);

      var result = await checker.Diagnose();

      Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Diagnose_ErrorOnConnect_ErrorResult()
    {
      var errorConnect = "Error_connect";

      var messageAdapter = new Mock<IMessageAdapter>();
      var messageAdapterConstructor = new Mock<IMessageAdapterConstructor>();
      messageAdapter.Setup(x => x.Configuration)
                    .Returns(CreateConnection());
      messageAdapter.Setup(x => x.Connect())
                     .Throws(new Exception(errorConnect));
      messageAdapter.Setup(x => x.Disconnect());

      messageAdapterConstructor.Setup(x => x.CreateAll(It.IsAny<bool>()))
                                .Returns(new[] { messageAdapter.Object });

      var checker = new MessagingConnectionChecker(new[] { messageAdapterConstructor.Object });

      var result = await checker.Diagnose();

      var diagnosticInfo = result.First();

      Assert.Multiple(() =>
      {
        Assert.That(diagnosticInfo.Error.Contains(errorConnect), Is.True);
        Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.QueueError));
        Assert.That(diagnosticInfo.IsSkipResult, Is.False);
        Assert.That(diagnosticInfo.Name, Is.EqualTo(MqSeriesId));
        Assert.That(diagnosticInfo.Url, Is.EqualTo(MqSeriesServer));
        Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      });
    }

    [Test]
    public async Task Diagnose_ErrorOnDisconnect_ErrorResult()
    {
      var errorConnect = "Error_connect";

      var messageAdapter = new Mock<IMessageAdapter>();
      var messageAdapterConstructor = new Mock<IMessageAdapterConstructor>();
      messageAdapter.Setup(x => x.Configuration)
                    .Returns(CreateConnection());
      messageAdapter.Setup(x => x.Connect());
      messageAdapter.Setup(x => x.Disconnect()).Throws(new Exception(errorConnect));

      messageAdapterConstructor.Setup(x => x.CreateAll(It.IsAny<bool>()))
                                .Returns(new[] { messageAdapter.Object });

      var checker = new MessagingConnectionChecker(new[] { messageAdapterConstructor.Object });

      var result = await checker.Diagnose();

      var diagnosticInfo = result.First();

      Assert.Multiple(() =>
      {
        Assert.That(diagnosticInfo.Error.Contains(errorConnect), Is.True);
        Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.QueueError));
        Assert.That(diagnosticInfo.IsSkipResult, Is.False);
        Assert.That(diagnosticInfo.Name, Is.EqualTo(MqSeriesId));
        Assert.That(diagnosticInfo.Url, Is.EqualTo(MqSeriesServer));
        Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      });
    }

    [Test]
    public async Task Diagnose_ErrorOnCreateAll_ErrorResult()
    {
      var errorConnect = "Error_connect";

      var messageAdapterConstructor = new Mock<IMessageAdapterConstructor>();

      messageAdapterConstructor.Setup(x => x.CreateAll(It.IsAny<bool>()))
                               .Throws(new Exception(errorConnect));

      var checker = new MessagingConnectionChecker(new[] { messageAdapterConstructor.Object });

      var result = await checker.Diagnose();

      var diagnosticInfo = result.First();

      Assert.Multiple(() =>
      {
        Assert.That(diagnosticInfo.Error.Contains(errorConnect), Is.True);
        Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.ConfigError));
        Assert.That(diagnosticInfo.IsSkipResult, Is.False);
        Assert.That(diagnosticInfo.Name, Is.EqualTo("constructor"));
        Assert.That(diagnosticInfo.Url, Is.Null);
        Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      });
    }
  }
}