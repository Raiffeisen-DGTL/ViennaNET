using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Logging;
using ViennaNET.Logging.Configuration;
using ViennaNET.Sagas.SagaDefinition;

namespace ViennaNET.Sagas.Tests.Unit
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(SagaBase<>))]
  public class SagaTests
  {
    [OneTimeSetUp]
    public void SetUp()
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

    [Test]
    public async Task Execute_SuccessSteps_CorrectStepsCallOrder()
    {
      // arrange
      var fakeService = new FakeService();
      var context = new FakeContext();
      var saga = new TestSuccessSaga(fakeService);

      // act
      Logger.LogInfo("Execute_SuccessSteps_CorrectStepsCallOrder");
      await saga.Execute(context);

      // assert
      Assert.That(fakeService._callOrder, Is.EquivalentTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public async Task Execute_FailedSteps_CorrectStepsCallOrder()
    {
      // arrange
      var fakeService = new FakeService();
      var context = new FakeContext();
      var saga = new TestFailedSaga(fakeService);

      // act
      Logger.LogInfo("Execute_FailedSteps_CorrectStepsCallOrder");
      await saga.Execute(context);

      // assert
      Assert.That(fakeService._callOrder, Is.EquivalentTo(new[] { 1, 2, 2, 1 }));
    }

    [Test]
    public async Task Execute_FailedStepsAfterTurning_CorrectStepsCallOrder()
    {
      // arrange
      var fakeService = new FakeService();
      var context = new FakeContext();
      var saga = new TestFailedSagaWithTurningStep(fakeService);

      // act
      Logger.LogInfo("Execute_FailedStepsAfterTurning_CorrectStepsCallOrder");
      await saga.Execute(context);

      // assert
      Assert.That(fakeService._callOrder, Is.EquivalentTo(new[] { 1, 2 }));
    }

    private class FakeContext
    {
      public int? ValuableValue { get; private set; }

      public void SetValuableValue(int? value)
      {
        ValuableValue = value;
      }
    }

    private class FakeService
    {
      public readonly List<int> _callOrder = new List<int>();

      public void Method1()
      {
        _callOrder.Add(1);
      }

      public void Method2()
      {
        _callOrder.Add(2);
      }

      public void Method3()
      {
        _callOrder.Add(3);
      }
    }

    private class TestSuccessSaga : SagaBase<FakeContext>
    {
      public TestSuccessSaga(FakeService fakeService)
      {
        Step("step 1@")
          .WithAction(c => fakeService.Method1());

        Step("step 2@")
          .WithAction(c => fakeService.Method2());

        Step("step 3@")
          .WithAction(c => fakeService.Method3());
      }
    }

    private class TestFailedSaga : SagaBase<FakeContext>
    {
      public TestFailedSaga(FakeService fakeService)
      {
        Step("step 1#")
          .WithAction(c => fakeService.Method1())
          .WithCompensation(c => fakeService.Method1());

        Step("step 2#")
          .WithAction(c => fakeService.Method2())
          .WithCompensation(c => fakeService.Method2());

        Step("step 3#")
          .WithAction(c => throw new AbortSagaExecutingException())
          .WithCompensation(c => fakeService.Method3());
      }
    }

    private class TestFailedSagaWithTurningStep : SagaBase<FakeContext>
    {
      public TestFailedSagaWithTurningStep(FakeService fakeService) : base("TestFailedSagaWithTurningStep")
      {
        Step("step 1")
          .WithAction(c => fakeService.Method1())
          .WithCompensation(c => fakeService.Method1());

        Step("step 2", StepType.Turning)
          .WithAction(c => fakeService.Method2())
          .WithCompensation(c => fakeService.Method2());

        Step("step 3")
          .WithAction(c => throw new AbortSagaExecutingException())
          .WithCompensation(c => fakeService.Method3());
      }
    }
  }
}
