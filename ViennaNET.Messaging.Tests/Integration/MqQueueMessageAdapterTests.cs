using System;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using ViennaNET.Logging;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue;

namespace ViennaNET.Messaging.Tests.Integration
{
  [TestFixture(Category = "Integration", TestOf = typeof(MqSeriesQueueMessageAdapter), Explicit = true)]
  public class MqQueueMessageAdapterTests
  {
    private readonly MqSeriesQueueConfiguration _configuration = new MqSeriesQueueConfiguration
    {
      Id = "test.queue",
      ClientId = "test.system",
      QueueManager = "TestManager",
      Server = "test.server",
      Port = 1414,
      Channel = "test.channel",
      ReplyQueue = "test.reply",
      QueueName = "test.queue",
      User = "",
      Password = "",
      QueueString = "queue:///TEST.QUEUE",
      TransactionEnabled = false,
      UseQueueString = true,
      Lifetime = TimeSpan.Parse("6:12")
    };

    [Test]
    public void CorrelationId_Test()
    {
      var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
        new XElement("mess", new XElement("messtype", "OPUstart"), new XElement("starttype", "2")));

      var message = new TextMessage();
      message.WriteXml(doc, Encoding.UTF8);

      string correlationID;
      using (var adapter = new MqSeriesQueueMessageAdapter(_configuration))
      {
        adapter.Connect();
        correlationID = adapter.Send(message)
          .CorrelationId;
        adapter.Disconnect();
      }

      using (var adapter = new MqSeriesQueueMessageAdapter(_configuration))
      {
        adapter.Connect();
        try
        {
          adapter.Receive(correlationID);
        }
        catch (MessagingException ex)
        {
          Logger.LogError(ex, string.Empty);
        }
        finally
        {
          adapter.Disconnect();
        }
      }
    }

    [Test]
    public void MQSeriesMessageAdapter_ConnectDisconnect()
    {
      using var adapter = new MqSeriesQueueMessageAdapter(_configuration);
      adapter.Connect();
      adapter.Disconnect();
    }
  }
}