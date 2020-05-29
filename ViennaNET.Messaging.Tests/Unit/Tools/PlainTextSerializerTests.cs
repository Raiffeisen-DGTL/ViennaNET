using System;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Unit.Tools
{
  [TestFixture, Category("Unit"), TestOf(typeof(PlainTextSerializer))]
  public class PlainTextSerializerTests
  {
    private const string MessageText = "Text";

    [Test]
    public void Serialize_CorrectText_TextMessageCreated()
    {
      var serializer = new PlainTextSerializer();

      var result = serializer.Serialize(MessageText);

      var message = (TextMessage)result;
      Assert.That(message.Body == MessageText);
    }

    [Test]
    public void Deserialize_TextMessage_CorrectResult()
    {
      var serializer = new PlainTextSerializer();

      var result = serializer.Deserialize(new TextMessage{Body = MessageText});

      Assert.That(result == MessageText);
    }

    [Test]
    public void Deserialize_BytesMessage_Exception()
    {
      var serializer = new PlainTextSerializer();

      Assert.Throws<InvalidCastException>(() => serializer.Deserialize(new BytesMessage()));
    }
  }
}
