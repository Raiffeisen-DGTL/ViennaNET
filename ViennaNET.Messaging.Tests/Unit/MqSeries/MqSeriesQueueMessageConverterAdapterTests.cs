using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IBM.XMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueMessageConverter))]
  public class MqSeriesQueueMessageConverterTests
  {
    private class TestDestination : IDestination
    {
      public bool GetBooleanProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public byte GetByteProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public short GetSignedByteProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public byte[] GetBytesProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public char GetCharProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public double GetDoubleProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public float GetFloatProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public int GetIntProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public long GetLongProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public object GetObjectProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public short GetShortProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public string GetStringProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public void SetBooleanProperty(string property_name, bool value)
      {
      }

      public void SetByteProperty(string property_name, byte value)
      {
      }

      public void SetSignedByteProperty(string property_name, short value)
      {
      }

      public void SetBytesProperty(string property_name, byte[] value)
      {
      }

      public void SetCharProperty(string property_name, char value)
      {
      }

      public void SetDoubleProperty(string property_name, double value)
      {
      }

      public void SetFloatProperty(string property_name, float value)
      {
      }

      public void SetIntProperty(string property_name, int value)
      {
      }

      public void SetLongProperty(string property_name, long value)
      {
      }

      public void SetObjectProperty(string property_name, object value)
      {
      }

      public void SetShortProperty(string property_name, short value)
      {
      }

      public void SetStringProperty(string property_name, string value)
      {
      }

      public bool PropertyExists(string propertyName)
      {
        throw new NotImplementedException();
      }

      public void Dispose()
      {
      }

      public DestinationType TypeId { get; }
      public string Name => "Test";
    }

    private class TextMessage : ITextMessage
    {
      public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

      public bool GetBooleanProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public byte GetByteProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public short GetSignedByteProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public byte[] GetBytesProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public char GetCharProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public double GetDoubleProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public float GetFloatProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public int GetIntProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public long GetLongProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public object GetObjectProperty(string property_name)
      {
        switch (property_name)
        {
          case "Param1":
            return 12;
          case "Param2":
            return "def";
          default:
            return null;
        }
      }

      public short GetShortProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public string GetStringProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public void SetBooleanProperty(string property_name, bool value)
      {
      }

      public void SetByteProperty(string property_name, byte value)
      {
      }

      public void SetSignedByteProperty(string property_name, short value)
      {
      }

      public void SetBytesProperty(string property_name, byte[] value)
      {
      }

      public void SetCharProperty(string property_name, char value)
      {
      }

      public void SetDoubleProperty(string property_name, double value)
      {
      }

      public void SetFloatProperty(string property_name, float value)
      {
      }

      public void SetIntProperty(string property_name, int value)
      {
      }

      public void SetLongProperty(string property_name, long value)
      {
      }

      public void SetObjectProperty(string property_name, object value)
      {
        Properties.Add(property_name, value);
      }

      public void SetShortProperty(string property_name, short value)
      {
      }

      public void SetStringProperty(string property_name, string value)
      {
      }

      public bool PropertyExists(string propertyName)
      {
        throw new NotImplementedException();
      }

      public void Acknowledge()
      {
      }

      public void ClearBody()
      {
      }

      public void ClearProperties()
      {
      }

      public string JMSCorrelationID { get; set; }
      public byte[] JMSCorrelationIDAsBytes { get; set; }
      public DeliveryMode JMSDeliveryMode { get; set; }
      public IDestination JMSDestination { get; set; }
      public long JMSExpiration { get; set; }
      public string JMSMessageID { get; set; }
      public int JMSPriority { get; set; }
      public bool JMSRedelivered { get; set; }
      public IDestination JMSReplyTo { get; set; }
      public long JMSTimestamp { get; set; }
      public string JMSType { get; set; }
      public IEnumerator PropertyNames => new List<string> { "Param1", "Param2" }.GetEnumerator();

      public string Text { get; set; }
    }

    private class BytesMessage : IBytesMessage
    {
      public byte[] Body { get; private set; }

      public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

      public bool GetBooleanProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public byte GetByteProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public short GetSignedByteProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public byte[] GetBytesProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public char GetCharProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public double GetDoubleProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public float GetFloatProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public int GetIntProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public long GetLongProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public object GetObjectProperty(string property_name)
      {
        switch (property_name)
        {
          case "Param1":
            return 12;
          case "Param2":
            return "def";
          default:
            return null;
        }
      }

      public short GetShortProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public string GetStringProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public void SetBooleanProperty(string property_name, bool value)
      {
      }

      public void SetByteProperty(string property_name, byte value)
      {
      }

      public void SetSignedByteProperty(string property_name, short value)
      {
      }

      public void SetBytesProperty(string property_name, byte[] value)
      {
      }

      public void SetCharProperty(string property_name, char value)
      {
      }

      public void SetDoubleProperty(string property_name, double value)
      {
      }

      public void SetFloatProperty(string property_name, float value)
      {
      }

      public void SetIntProperty(string property_name, int value)
      {
      }

      public void SetLongProperty(string property_name, long value)
      {
      }

      public void SetObjectProperty(string property_name, object value)
      {
        Properties.Add(property_name, value);
      }

      public void SetShortProperty(string property_name, short value)
      {
      }

      public void SetStringProperty(string property_name, string value)
      {
      }

      public bool PropertyExists(string propertyName)
      {
        throw new NotImplementedException();
      }

      public void Acknowledge()
      {
      }

      public void ClearBody()
      {
      }

      public void ClearProperties()
      {
      }

      public string JMSCorrelationID { get; set; }
      public byte[] JMSCorrelationIDAsBytes { get; set; }
      public DeliveryMode JMSDeliveryMode { get; set; }
      public IDestination JMSDestination { get; set; }
      public long JMSExpiration { get; set; }
      public string JMSMessageID { get; set; }
      public int JMSPriority { get; set; }
      public bool JMSRedelivered { get; set; }
      public IDestination JMSReplyTo { get; set; }
      public long JMSTimestamp { get; set; }
      public string JMSType { get; set; }
      public IEnumerator PropertyNames => new List<string> { "Param1", "Param2" }.GetEnumerator();

      public void Reset()
      {
      }

      public bool ReadBoolean()
      {
        throw new NotImplementedException();
      }

      public byte ReadByte()
      {
        throw new NotImplementedException();
      }

      public short ReadSignedByte()
      {
        throw new NotImplementedException();
      }

      public int ReadBytes(byte[] array)
      {
        array[0] = 97;
        array[1] = 98;
        array[2] = 99;

        return 3;
      }

      public int ReadBytes(byte[] array, int length)
      {
        throw new NotImplementedException();
      }

      public char ReadChar()
      {
        throw new NotImplementedException();
      }

      public double ReadDouble()
      {
        throw new NotImplementedException();
      }

      public float ReadFloat()
      {
        throw new NotImplementedException();
      }

      public int ReadInt()
      {
        throw new NotImplementedException();
      }

      public long ReadLong()
      {
        throw new NotImplementedException();
      }

      public short ReadShort()
      {
        throw new NotImplementedException();
      }

      public string ReadUTF()
      {
        throw new NotImplementedException();
      }

      public int ReadUnsignedShort()
      {
        throw new NotImplementedException();
      }

      public void WriteBoolean(bool value)
      {
      }

      public void WriteByte(byte value)
      {
      }

      public void WriteSignedByte(short value)
      {
      }

      public void WriteBytes(byte[] value)
      {
        Body = value;
      }

      public void WriteBytes(byte[] value, int offset, int length)
      {
      }

      public void WriteChar(char value)
      {
      }

      public void WriteDouble(double value)
      {
      }

      public void WriteFloat(float value)
      {
      }

      public void WriteInt(int value)
      {
      }

      public void WriteLong(long value)
      {
      }

      public void WriteObject(object value)
      {
      }

      public void WriteShort(short value)
      {
      }

      public void WriteUTF(string value)
      {
      }

      public long BodyLength => 3;
    }

    private class UnknownMessage : IMessage
    {
      public bool GetBooleanProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public byte GetByteProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public short GetSignedByteProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public byte[] GetBytesProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public char GetCharProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public double GetDoubleProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public float GetFloatProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public int GetIntProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public long GetLongProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public object GetObjectProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public short GetShortProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public string GetStringProperty(string property_name)
      {
        throw new NotImplementedException();
      }

      public void SetBooleanProperty(string property_name, bool value)
      {
        throw new NotImplementedException();
      }

      public void SetByteProperty(string property_name, byte value)
      {
        throw new NotImplementedException();
      }

      public void SetSignedByteProperty(string property_name, short value)
      {
        throw new NotImplementedException();
      }

      public void SetBytesProperty(string property_name, byte[] value)
      {
        throw new NotImplementedException();
      }

      public void SetCharProperty(string property_name, char value)
      {
        throw new NotImplementedException();
      }

      public void SetDoubleProperty(string property_name, double value)
      {
        throw new NotImplementedException();
      }

      public void SetFloatProperty(string property_name, float value)
      {
        throw new NotImplementedException();
      }

      public void SetIntProperty(string property_name, int value)
      {
        throw new NotImplementedException();
      }

      public void SetLongProperty(string property_name, long value)
      {
        throw new NotImplementedException();
      }

      public void SetObjectProperty(string property_name, object value)
      {
        throw new NotImplementedException();
      }

      public void SetShortProperty(string property_name, short value)
      {
        throw new NotImplementedException();
      }

      public void SetStringProperty(string property_name, string value)
      {
        throw new NotImplementedException();
      }

      public bool PropertyExists(string propertyName)
      {
        throw new NotImplementedException();
      }

      public void Acknowledge()
      {
        throw new NotImplementedException();
      }

      public void ClearBody()
      {
        throw new NotImplementedException();
      }

      public void ClearProperties()
      {
        throw new NotImplementedException();
      }

      public string JMSCorrelationID { get; set; }
      public byte[] JMSCorrelationIDAsBytes { get; set; }
      public DeliveryMode JMSDeliveryMode { get; set; }
      public IDestination JMSDestination { get; set; }
      public long JMSExpiration { get; set; }
      public string JMSMessageID { get; set; }
      public int JMSPriority { get; set; }
      public bool JMSRedelivered { get; set; }
      public IDestination JMSReplyTo { get; set; }
      public long JMSTimestamp { get; set; }
      public string JMSType { get; set; }
      public IEnumerator PropertyNames => new List<string>().GetEnumerator();
    }

    private static Mock<ISession> CreateTextSession()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.CreateTextMessage(It.IsAny<string>()))
             .Returns((Func<string, TextMessage>)(x => new TextMessage { Text = x }));
      return session;
    }

    private class UnknownBaseMessage : BaseMessage
    {
      public override string LogBody()
      {
        throw new NotImplementedException();
      }

      public override bool IsEmpty()
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void ConvertToBaseMessage_CorrectBytesMessage_CorrectResult()
    {
      var message = new BytesMessage();

      var result = message.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        var bytesMessage = (Messaging.Messages.BytesMessage)result;
        Assert.That(bytesMessage.Body[0] == 97);
        Assert.That(bytesMessage.Body[1] == 98);
        Assert.That(bytesMessage.Body[2] == 99);
      });
    }

    [Test]
    public void ConvertToBaseMessage_CorrectTextMessage_CorrectResult()
    {
      var message = new TextMessage
      {
        JMSTimestamp = 23456,
        JMSExpiration = 23789,
        Text = "abc",
        JMSMessageID = "1234",
        JMSCorrelationID = "7891",
        JMSReplyTo = new TestDestination()
      };

      var result = message.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        var textMessage = (Messaging.Messages.TextMessage)result;
        Assert.That(textMessage.Body == "abc");
        Assert.That(textMessage.ApplicationTitle, Is.Null);
        Assert.That(textMessage.CorrelationId == "7891");
        Assert.That(textMessage.LifeTime == TimeSpan.FromMilliseconds(333));
        Assert.That(textMessage.MessageId == "1234");
        var prop1 = textMessage.Properties.First();
        var prop2 = textMessage.Properties.Last();
        Assert.That(prop1.Key == "Param1");
        Assert.That(prop1.Value, Is.EqualTo(12));
        Assert.That(prop2.Key == "Param2");
        Assert.That(prop2.Value, Is.EqualTo("def"));
        Assert.That(textMessage.ReplyQueue == "Test");
        Assert.That(textMessage.SendDateTime == new DateTime(1970, 1, 1, 0, 0, 23, 456));
      });
    }

    [Test]
    public void ConvertToBaseMessage_SendDateNewerThanExpirationDate_ZeroLifeTime()
    {
      var message = new TextMessage
      {
        JMSTimestamp = 23789,
        JMSExpiration = 23345,
        Text = "abc",
        JMSMessageID = "1234",
        JMSCorrelationID = "7891",
        JMSReplyTo = new TestDestination()
      };

      var result = message.ConvertToBaseMessage();

      Assert.That(result.LifeTime == TimeSpan.Zero);
    }

    [Test]
    public void ConvertToBaseMessage_UnknownMessage_TextMessageCreated()
    {
      var message = new UnknownMessage();

      var result = message.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        var textMessage = (Messaging.Messages.TextMessage)result;
        Assert.That(textMessage.Body, Is.Empty);
      });
    }

    [Test]
    public void ConvertToMqMessage_CorrectBytesMessage_CorrectResult()
    {
      var reply = new TestDestination();
      var session = new Mock<ISession>();
      session.Setup(x => x.CreateBytesMessage())
             .Returns(new BytesMessage());
      session.Setup(x => x.CreateQueue(It.IsAny<string>()))
             .Returns(reply);
      var message = new Messaging.Messages.BytesMessage
      {
        Body = new byte[] { 97, 98, 99 },
        MessageId = "abc",
        CorrelationId = "def",
        LifeTime = TimeSpan.FromMinutes(10),
        ReplyQueue = "Reply",
        Properties = { { "Prop1", 12 }, { "Prop2", "def" } }
      };

      var result = message.ConvertToMqMessage(session.Object);

      Assert.Multiple(() =>
      {
        var bytesMessage = (BytesMessage)result;
        var bytes = new byte[3];
        bytesMessage.ReadBytes(bytes);
        Assert.That(bytes[0] == 97);
        Assert.That(bytes[1] == 98);
        Assert.That(bytes[2] == 99);
        Assert.That(bytesMessage.JMSMessageID == "abc");
        Assert.That(bytesMessage.JMSExpiration == 6000);
        Assert.That(bytesMessage.JMSCorrelationID == "def");
        Assert.That(bytesMessage.JMSReplyTo, Is.EqualTo(reply));
        var prop1 = bytesMessage.Properties.First();
        var prop2 = bytesMessage.Properties.Last();
        Assert.That(prop1.Key == "Prop1");
        Assert.That(prop1.Value, Is.EqualTo(12));
        Assert.That(prop2.Key == "Prop2");
        Assert.That(prop2.Value, Is.EqualTo("def"));
      });
    }

    [Test]
    public void ConvertToMqMessage_CorrectTextMessage_CorrectResult()
    {
      var session = CreateTextSession();
      var message = new Messaging.Messages.TextMessage
      {
        Body = "Body", MessageId = "abc", CorrelationId = "def", LifeTime = TimeSpan.FromMinutes(10)
      };

      var result = message.ConvertToMqMessage(session.Object);

      var textMessage = (TextMessage)result;
      Assert.That(textMessage.Text == "Body");
    }

    [Test]
    public void ConvertToMqMessage_EmptyCorrId_MessageIdUsed()
    {
      var session = CreateTextSession();
      var message = new Messaging.Messages.TextMessage
      {
        Body = "Body", MessageId = "abc", CorrelationId = null, LifeTime = TimeSpan.FromMinutes(10)
      };

      var result = message.ConvertToMqMessage(session.Object);

      Assert.That(result.JMSCorrelationID == message.MessageId);
    }

    [Test]
    public void ConvertToMqMessage_EmptyMessageId_GuidGenerated()
    {
      var session = CreateTextSession();
      var message = new Messaging.Messages.TextMessage
      {
        Body = "Body", MessageId = null, CorrelationId = "def", LifeTime = TimeSpan.FromMinutes(10)
      };

      var result = message.ConvertToMqMessage(session.Object);

      Assert.That(Guid.TryParse(result.JMSMessageID, out _));
    }

    [Test]
    public void ConvertToMqMessage_UnknownMessageType_Exception()
    {
      var session = CreateTextSession();
      var message = new UnknownBaseMessage();

      Assert.Throws<ArgumentException>(() => message.ConvertToMqMessage(session.Object));
    }
  }
}