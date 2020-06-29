using System;
using System.Text.Json;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Unit.Tools
{
  [TestFixture(Category = "Unit")]
  [TestOf(typeof(JsonMessageSerializer<>))]
  public class JsonMessageSerializerTests
  {
    [Test]
    public void Serialize_HasObject_Serialized()
    {
      // arrange
      var objToSerialize = new TestBody() { Field = 1 };

      // act
      var serializer = new JsonMessageSerializer<TestBody>();
      var result = serializer.Serialize(objToSerialize);

      // assert
      var expectedJson = JsonSerializer.Serialize(objToSerialize);
      Assert.That(result, Is.TypeOf(typeof(TextMessage)));
      Assert.That(((TextMessage)result).Body, Is.EqualTo(expectedJson));
    }

    [Test]
    public void Deserialize_TextMessageHasBody_Deserialize()
    {
      // arrange
      var expectedBody = new TestBody() { Field = 1 };
      var message = new TextMessage()
      {
        Body = JsonSerializer.Serialize(expectedBody)
      };

      // act
      var serializer = new JsonMessageSerializer<TestBody>();
      var result = serializer.Deserialize(message);

      // assert
      Assert.That(result, Is.TypeOf(typeof(TestBody)));
      Assert.That(result.Field, Is.EqualTo(1));
    }

    [Test]
    public void Deserialize_NotATextMessage_ThrowsInvalidCastException()
    {
      // arrange
      var message = new BytesMessage();

      // act
      var serializer = new JsonMessageSerializer<TestBody>();

      // assert
      Assert.Throws<InvalidCastException>(() => serializer.Deserialize(message));
    }

    private class TestBody
    {
      public int Field { get; set; }
    }
  }
}

