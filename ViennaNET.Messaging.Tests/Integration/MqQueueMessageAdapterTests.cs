using System;
using System.Text;
using System.Xml.Linq;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue;
using NUnit.Framework;
using System.Collections.Generic;

namespace ViennaNET.Messaging.Tests.Integration
{
  [Explicit]
  [TestFixture(Category = "Integration", TestOf = typeof(MqSeriesQueueMessageAdapterBase))]
  public class MqQueueMessageAdapterTests
  {
    private static readonly MqSeriesQueueConfiguration _configuration = new MqSeriesQueueConfiguration
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

    public static IEnumerable<Type> GetAdapterTypes()
    {
      return new[]
      {
        typeof(MqSeriesQueueSubscribingMessageAdapter),
        typeof(MqSeriesQueueTransactedMessageAdapter)
      };
    }

    private MqSeriesQueueMessageAdapterBase CreateAdapter(Type type)
    {
      return (MqSeriesQueueMessageAdapterBase)type.GetConstructor(new[] { typeof(MqSeriesQueueConfiguration) }).Invoke(new[] { _configuration });
    }

    [Test]
    public void MQSeriesMessageAdapter_ConnectDisconnect([ValueSource("GetAdapterTypes")]Type adapterType)
    {
      using (
        var adapter = CreateAdapter(adapterType))
      {
        adapter.Connect();
        adapter.Disconnect();
      }
    }

    [Test]
    public void CorrelationId_Test([ValueSource("GetAdapterTypes")] Type adapterType)
    {
      var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                              new XElement("mess", new XElement("messtype", "OPUstart"), new XElement("starttype", "2")));

      var message = new TextMessage();
      message.WriteXml(doc, Encoding.UTF8);

      string correlationID;
      using (var adapter = CreateAdapter(adapterType))
      {
        adapter.Connect();
        correlationID = adapter.Send(message)
                               .CorrelationId;
        adapter.Disconnect();
      }

      using (var adapter = CreateAdapter(adapterType))
      {
        adapter.Connect();
        try
        {
          adapter.Receive(correlationID);
        }
        catch (MessagingException ex)
        {
          Assert.Fail(ex.ToString());
        }
        finally
        {
          adapter.Disconnect();
        }
      }
    }
  }
}
