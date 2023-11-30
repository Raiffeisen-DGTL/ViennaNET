using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace ViennaNET.ArcSight.Tests.Unit
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(CefMessageSerializer))]
  public class CefMessageSerializerTests
  {
    public static object[] MessageCases()
    {
      var startTime = DateTimeOffset.Parse("2013-09-19 08:26:10.999");
      var hostName = "host";
      return new object[]
      {
        new object[]
        {
          new CefMessage(startTime, hostName, "Security", "threatmanager", "1.0", 100, "worm successfullystopped",
            CefSeverity.Emergency),
          "Sep 19 08:26:10 host CEF:0|Security|threatmanager|1.0|100|worm successfullystopped|10|shost=host start=Sep 19 2013 08:26:10"
        },
        new object[]
        {
          new CefMessage(startTime, hostName, "Security", "threatmanager", "1.0", 100, "тест тест",
            CefSeverity.Emergency),
          "Sep 19 08:26:10 host CEF:0|Security|threatmanager|1.0|100|тест тест|10|shost=host start=Sep 19 2013 08:26:10"
        }
      };
    }

    [Test]
    [TestCaseSource(nameof(MessageCases))]
    public void CefMessageCasesTest(CefMessage cefMessage, string expectedMessage)
    {
      var serializer = new CefMessageSerializer(new SyslogRfc3164MessageSerializer());

      string result;
      using (var stream = new MemoryStream())
      {
        var syslogMessage = serializer.Serialize(cefMessage);
        serializer.Serialize(syslogMessage, stream);
        stream.Flush();
        stream.Position = 0;

        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
          result = reader.ReadLine();
        }
      }

      Assert.That(result, Is.EqualTo($"<8>{expectedMessage}"));
    }
  }
}