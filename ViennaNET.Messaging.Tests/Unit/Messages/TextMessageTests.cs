using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tests.Unit.Messages
{
  [TestFixture(Category = "Unit", TestOf = typeof(TextMessage))]
  public class TextMessageTests
  {
    private static TextMessage CreateTextMessage(string body)
    {
      var message = new TextMessage
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
          yield return new TestCaseData("testMessage", "testMessage");
          yield return new TestCaseData("", "");
          yield return new TestCaseData(null, null);
        }
      }
    }

    [Test]
    [TestCaseSource(typeof(TestDataProvider), nameof(TestDataProvider.Bodies))]
    public void LogBody_MessageBodyHasBeenFilled_CorrectlyReturned(string source, string body)
    {
      var message = CreateTextMessage(source);

      var result = message.LogBody();

      Assert.That(result == body);
    }

    [Test]
    public void ToString_MessageHasBeenFilled_CorrectlySerializedWithoutProperties()
    {
      var xml = new StreamReader(GetType()
                                 .Assembly.GetManifestResourceStream("ViennaNET.Messaging.Tests.Unit.Messages.TextMessage.xml")
                                 ?? new MemoryStream()).ReadToEnd();
      var message = CreateTextMessage("testMessage");

      var result = message.ToString();

      Assert.That(result, Is.Not.Null);
    }
  }
}