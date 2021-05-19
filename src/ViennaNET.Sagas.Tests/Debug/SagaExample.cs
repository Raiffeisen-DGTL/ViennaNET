using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using ViennaNET.Logging;
using ViennaNET.Logging.Configuration;
using ViennaNET.Sagas.SagaDefinition;

namespace ViennaNET.Sagas.Tests.Debug
{
  [TestFixture]
  [Category("Debug")]
  [Explicit]
  public class SagaExample
  {
    [SetUp]
    public void ConfigureLogger()
    {
      var configuration = new ConfigurationBuilder().AddInMemoryCollection(new[]
                                                    {
                                                      new KeyValuePair<string, string
                                                      >("logger:listeners:listeners0:type", "console"),
                                                      new KeyValuePair<string, string
                                                      >("logger:listeners:listeners0:category", "All"),
                                                      new KeyValuePair<string, string
                                                      >("logger:listeners:listeners0:minLevel", "Debug")
                                                    })
                                                    .Build();

      Logger.Configure(new LoggerJsonCfgFileConfiguration(configuration));
    }

    private class FakeContext
    {
      public int? ValuableValue { get; private set; }

      public void SetValuableValue(int? value)
      {
        ValuableValue = value;
      }
    }

    private class TestSaga : SagaBase<FakeContext>
    {
      public TestSaga()
      {
        Step("hamster step 1")
          .WithAction(c => Logger.LogInfo($"action hamster 1. {JsonConvert.SerializeObject(c)}"))
          .WithCompensation(c => Logger.LogInfo($"compensation hamster 1. {JsonConvert.SerializeObject(c)}"));

        AsyncStep("hamster step 2")
          .WithAction(HamsterStep2Action)
          .WithCompensation(HamsterStep2Compensation);

        Step("hamster 3")
          .WithCompensation(context => Logger.LogInfo("compensation without action hamster3"));

        Step("achtung hamster 4!")
          .WithAction(context => throw new AbortSagaExecutingException());

        Step("hamster5")
          .WithAction(context => Logger.LogInfo("action hamster5"))
          .WithCompensation(context => Logger.LogInfo("compensation hamster5"));
      }

      private async Task HamsterStep2Action(FakeContext context)
      {
        context.SetValuableValue(2);
        Logger.LogInfo($"action hamster 2. {JsonConvert.SerializeObject(context)}");
      }

      private async Task HamsterStep2Compensation(FakeContext context)
      {
        context.SetValuableValue(null);
        Logger.LogInfo($"compensation hamster 2. {JsonConvert.SerializeObject(context)}");
      }
    }

    [Test]
    public async Task BuilderTest2()
    {
      var context = new FakeContext();

      var saga = new TestSaga();

      await saga.Execute(context);
    }
  }
}
