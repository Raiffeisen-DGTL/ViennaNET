using NUnit.Framework;
using SyslogNet.Client.Transport;

namespace ViennaNET.ArcSight.Tests.Integration
{
    [TestFixture(Category = "Debug")]
  public class TestClient
  {
    [Test]
    [Explicit]
    public void CefSender_SendCef()
    {
      var serializer = new CefMessageSerializer(new SyslogRfc3164MessageSerializer());
      using (var sender = new CefSender(new SyslogTcpSender("127.0.0.1", 514)))
      {
        var cef = new CefMessage(DateTimeOffset.UtcNow, "host", "Raiff", "ICDB", "1.1", 555, "test", CefSeverity.Error);

        Assert.That(() => sender.Send(cef, serializer), Throws.Nothing);
      }
    }
  }
}