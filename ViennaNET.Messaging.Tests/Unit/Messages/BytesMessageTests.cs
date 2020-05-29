using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tests.Unit.Messages
{
  [TestFixture(Category = "Unit", TestOf = typeof(BytesMessage))]
  public class BytesMessageTests
  {
    private static BytesMessage CreateBytesMessage(byte[] body)
    {
      var message = new BytesMessage
      {
        ApplicationTitle = "Title",
        Body = body,
        CorrelationId = "corrId",
        LifeTime = TimeSpan.MinValue,
        MessageId = "messId",
        ReceiveDate = new DateTime(2014, 10, 19),
        ReplyQueue = "replyQueue",
        SendDateTime = new DateTime(2014, 10, 18),
        Properties = { { "key", "value" } }
      };
      return message;
    }

    private class TestDataProvider
    {
      public static IEnumerable<TestCaseData> Bodies
      {
        get
        {
          yield return new TestCaseData("testMessage", "bytes buffer with length = 11");
          yield return new TestCaseData("", "empty bytes buffer");
          yield return new TestCaseData(null, "empty bytes buffer");
        }
      }
    }

    [Test]
    [TestCaseSource(typeof(TestDataProvider), nameof(TestDataProvider.Bodies))]
    public void LogBody_MessageBodyHasBeenFilled_CorrectlyReturned(string source, string body)
    {
      var message = CreateBytesMessage(string.IsNullOrEmpty(source)
                                         ? null
                                         : Encoding.UTF8.GetBytes(source));

      var result = message.LogBody();

      Assert.That(result == body);
    }

    [Test]
    public void ToString_MessageHasBeenFilled_CorrectlySerializedWithoutProperties()
    {
      var xml = new StreamReader(GetType()
                                 .Assembly.GetManifestResourceStream("ViennaNET.Messaging.Tests.Unit.Messages.BytesMessage.xml")
                                 ?? new MemoryStream()).ReadToEnd();
      var message = CreateBytesMessage(Encoding.UTF8.GetBytes("testMessage"));

      var result = message.ToString();

      Assert.That(result, Is.Not.Null);
    }
  }
}