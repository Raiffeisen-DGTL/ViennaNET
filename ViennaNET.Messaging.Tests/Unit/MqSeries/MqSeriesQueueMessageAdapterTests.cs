using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.MQSeriesQueue;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueMessageAdapter))]
  public class MqSeriesQueueMessageAdapterTests
  {
    private MqSeriesQueueMessageAdapter _adapter;
    private MqSeriesQueueMessageAdapter _additionalAdapter;

    [OneTimeSetUp]
    public void MqSeriesQueueMessageAdapterConstructorSetup()
    {
      var configuration = new MqSeriesQueueConfiguration { Id = "id", Selector = "TEST = 'TEST'" };

      var additionalConfiguration =
        new MqSeriesQueueConfiguration { Id = "id2", Selector = "TEST = 'TEST' AND (TEST2=:TEST2 OR TEST3 = :TEST3)" };

      _adapter = new MqSeriesQueueMessageAdapter(configuration);
      _additionalAdapter = new MqSeriesQueueMessageAdapter(additionalConfiguration);
    }

    [Test]
    public void CreateSelector_HasInitialSelector_Selector()
    {
      var result = _adapter.CreateSelector("123", Enumerable.Empty<(string, string)>());

      Assert.That(result == "(JMSCorrelationID='123') AND TEST = 'TEST'");
    }

    [Test]
    public void CreateSelector_HasInitialSelectorAndAdditionalParams_Selector()
    {
      var result = _additionalAdapter.CreateSelector("123", new[] { ("TEST2", "0x0B2"), ("TEST3", "abc") });

      Assert.That(result == "(JMSCorrelationID='123') AND TEST = 'TEST' AND (TEST2='0x0B2' OR TEST3 = 'abc')");
    }

    [Test]
    public void CreateSelector_HasSelectors_Selector()
    {
      var result = _adapter.CreateSelector(string.Empty, Enumerable.Empty<(string, string)>());

      Assert.That(result == "TEST = 'TEST'");
    }

    [Test]
    public void CreateSelector_NullInitialSelector_Selector()
    {
      var result = _adapter.CreateSelector(null, Enumerable.Empty<(string, string)>());

      Console.WriteLine(result);

      Assert.That(result == "TEST = 'TEST'");
    }

    [Test]
    public void CreateSelector_NullInitialSelectorAndAdditionalParams_Selector()
    {
      var result = _additionalAdapter.CreateSelector(null, new[] { ("TEST2", "0x0B2"), ("TEST3", "abc") });

      Assert.That(result == "TEST = 'TEST' AND (TEST2='0x0B2' OR TEST3 = 'abc')");
    }

    [Test]
    [TestCaseSource(nameof(TimeoutCases))]
    public void GetTimeout_TimeSpan_Result(TimeSpan? timespan, long timeout)
    {
      var result = MqSeriesQueueMessageAdapter.GetTimeout(timespan);

      Assert.That(result == timeout);
    }

    [Test]
    [TestCase(MessageProcessingType.ThreadStrategy, true)]
    [TestCase(MessageProcessingType.Subscribe, true)]
    [TestCase(MessageProcessingType.SubscribeAndReply, false)]
    public void SupportProcessingType_TypePassed_Result(MessageProcessingType type, bool res)
    {
      var result = _adapter.SupportProcessingType(type);

      Assert.That(result == res);
    }

    private static object[] TimeoutCases()
    {
      return new object[]
      {
        new object[]
        {
          Timeout.InfiniteTimeSpan, 0
        },
        new object[]
        {
          TimeSpan.MaxValue, 0
        },
        new object[]
        {
          TimeSpan.MinValue, -1
        },
        new object[]
        {
          null, -1
        },
        new object[]
        {
          TimeSpan.FromSeconds(10), 10000
        },
        new object[]
        {
          TimeSpan.FromSeconds(-10), -1
        },
        new object[]
        {
          TimeSpan.Zero, -1
        },
      };
    }
  }
}