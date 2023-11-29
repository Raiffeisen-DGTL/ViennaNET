using System;
using Moq;
using NUnit.Framework;
using SyslogNet.Client;
using SyslogNet.Client.Serialization;
using SyslogNet.Client.Transport;

namespace ViennaNET.ArcSight.Tests.Unit
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(CefSender))]
  public class CefSenderTests
  {
    private readonly CefSender _cefSender;
    private readonly Mock<ISyslogMessageSender> _syslogSenderMock;
    private readonly CefMessageSerializer _serializer;

    public CefSenderTests()
    {
      _syslogSenderMock = new Mock<ISyslogMessageSender>();
      _cefSender = new CefSender(_syslogSenderMock.Object);
      _serializer = new CefMessageSerializer(new Mock<ISyslogMessageSerializer>().Object);
    }

    [Test]
    public void Dispose_SimpleCall_SyslogSenderCalled()
    {
      _cefSender.Dispose();

      _syslogSenderMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Test]
    public void Reconnect_SimpleCall_SyslogSenderCalled()
    {
      _cefSender.Reconnect();

      _syslogSenderMock.Verify(x => x.Reconnect(), Times.Once);
    }

    [Test]
    public void Send_OneMessage_SyslogSenderCalled()
    {
      var message = new CefMessage(new DateTime(2016, 1, 1), "host", "Security", "threatmanager", "1.0", 100,
        "worm successfullystopped",
        CefSeverity.Emergency);

      _cefSender.Send(message, _serializer);

      _syslogSenderMock.Verify(x => x.Send(It.IsAny<SyslogMessage>(), It.IsAny<CefMessageSerializer>()), Times.Once);
    }

    [Test]
    public void Send_TwoMessages_SyslogSenderCalledTwice()
    {
      var firstMessage = new CefMessage(new DateTime(2016, 1, 1), "host", "Security", "threatmanager", "1.0", 100,
        "worm successfullystopped", CefSeverity.Emergency);

      var secondMessage = new CefMessage(new DateTime(2016, 1, 1), "host", "Security", "threatmanager", "1.0", 100,
        "worm successfullystopped", CefSeverity.Emergency);

      var serializer = new CefMessageSerializer(new Mock<ISyslogMessageSerializer>().Object);

      var syslogSenderMock = new Mock<ISyslogMessageSender>();
      var cefSender = new CefSender(syslogSenderMock.Object);
      cefSender.Send(new[] { firstMessage, secondMessage }, serializer);

      syslogSenderMock.Verify(x => x.Send(It.IsAny<SyslogMessage>(), It.IsAny<CefMessageSerializer>()),
        Times.Exactly(2));
    }
  }
}