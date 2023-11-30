using System;
using System.Text;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Unit.Tools
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(XmlMessageSerializer<>))]
  public class XmlMessageSerializerTests
  {
    private const string MessageText =
      @"<?xml version=""1.0"" encoding=""utf-8""?><TextMessage xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><SendDateTime xsi:nil=""true"" /><ReceiveDate xsi:nil=""true"" /><LifeTime>PT0S</LifeTime><Body>testMessage</Body></TextMessage>";

    [Test]
    public void Deserialize_BytesMessage_CastResult()
    {
      var serializer = new XmlMessageSerializer<TextMessage>();
      var message = new BytesMessage { Body = Encoding.UTF8.GetBytes(MessageText) };

      var result = serializer.Deserialize(message);

      Assert.That(result.Body == "testMessage");
    }

    [Test]
    public void Deserialize_BytesMessage_Exception()
    {
      var serializer = new XmlMessageSerializer<BytesMessage>();
      var message = new TextMessage { Body = MessageText };

      Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(message));
    }

    [Test]
    public void Deserialize_TextMessage_CorrectResult()
    {
      var serializer = new XmlMessageSerializer<TextMessage>();

      var result = serializer.Deserialize(new TextMessage { Body = MessageText });

      Assert.That(result.Body == "testMessage");
    }

    [Test]
    public void Serialize_CorrectText_TextMessageCreated()
    {
      var serializer = new XmlMessageSerializer<TextMessage>();

      var result = serializer.Serialize(new TextMessage { Body = "testMessage" });

      var message = (TextMessage)result;
      var m = message.Body.Replace("\r", "")
        .Replace("\n", "")
        .Replace(">  <", "><");
      Assert.That(m == MessageText);
    }
  }
}