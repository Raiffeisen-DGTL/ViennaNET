using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.MQSeriesQueue;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueMessageAdapter))]
  public class MqSeriesQueueMessageAdapterTests
  {
    private MqSeriesQueueMessageAdapter _adapter;

    [OneTimeSetUp]
    public void MqSeriesQueueMessageAdapterConstructorSetup()
    {
      var configuration = new MqSeriesQueueConfiguration { Id = "id", Selectors = new List<CustomHeader> { new CustomHeader { Key = "TEST", Value = "TEST" } } };

      _adapter = new MqSeriesQueueMessageAdapter(configuration);
    }

    [Test]
    public void AddSelectorFromConfig_HasSelectors_CombinedSelector()
    {
      var result = _adapter.AddSelectorFromConfig(string.Empty);

      Assert.That(result == "TEST = 'TEST'");
    }

    [Test]
    public void AddSelectorFromConfig_HasInitialSelector_CombinedSelector()
    {
      var result = _adapter.AddSelectorFromConfig("JmsCorId = '123'");

      Assert.That(result == "JmsCorId = '123' AND TEST = 'TEST'");
    }

    [Test]
    public void AddSelectorFromConfig_NullInitialSelector_CombinedSelector()
    {
      var result = _adapter.AddSelectorFromConfig(null);

      Assert.That(result == "TEST = 'TEST'");
    }
  }
}
