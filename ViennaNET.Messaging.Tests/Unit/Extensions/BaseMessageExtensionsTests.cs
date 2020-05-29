using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tests.Unit.Extensions
{
  [TestFixture(Category = "Unit", TestOf = typeof(BaseMessageExtensions))]
  public class BaseMessageExtensionsTests
  {
    private XDocument _xml;

    [OneTimeSetUp]
    public void Setup()
    {
      _xml = XDocument.Load(GetType()
                            .Assembly.GetManifestResourceStream("ViennaNET.Messaging.Tests.Unit.Extensions.XmlTestData.xml"));
    }

    [Test]
    public void WriteXml_CorrectXml_CorrectlyWritten()
    {
      var message = new TextMessage();

      message.WriteXml(new XDocument(_xml), Encoding.UTF8);

      using var stream = new MemoryStream();
      using var tw = new XmlTextWriter(stream, Encoding.UTF8);
      _xml.Save(tw);
      tw.Flush();
      stream.Position = 0;

      using var sr = new StreamReader(stream);
      var body = sr.ReadToEnd();
      Assert.That(body == message.Body);
    }

    [Test]
    public void ReadXml_CorrectXml_CorrectlyRead()
    {
      var message = new TextMessage { Body = _xml.ToString() };

      var document = message.ReadXml();

      Assert.That(document.ToString() == message.Body);
    }

    [Test]
    public void GetMessageBodyAsBytes_BytesMessage_BodyReturned()
    {
      var body = Encoding.UTF8.GetBytes(_xml.ToString());
      var message = new BytesMessage { Body = body };

      var result = message.GetMessageBodyAsBytes();

      Assert.That(result == body);
    }

    [Test]
    public void GetMessageBodyAsBytes_TextMessage_BodyReturned()
    {
      var body = Encoding.UTF8.GetBytes(_xml.ToString());
      var message = new TextMessage { Body = _xml.ToString() };

      var result = message.GetMessageBodyAsBytes();

      Assert.That(result.ToString().Equals(body.ToString()));
    }

    [Test]
    public void GetMessageBodyAsBytes_UnknownTypeMessage_ExceptionThrown()
    {
      var message = new TestMessage();

      Assert.Throws<MessagingException>(() => message.GetMessageBodyAsBytes());
    }

    private class TestMessage : BaseMessage
    {
      public override string LogBody()
      {
        throw new System.NotImplementedException();
      }

      public override bool IsEmpty()
      {
        throw new System.NotImplementedException();
      }
    }
  }
}