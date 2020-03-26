using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViennaNET.Logging;

namespace ViennaNET.Sagas.SagaDefinition
{
  /// <summary>
  /// Базовый класс для создания саг
  /// </summary>
  /// <typeparam name="TCont"></typeparam>
  public abstract class SagaBase<TCont> where TCont : class
  {
    /// <summary>
    /// Имя саги
    /// </summary>
    public string Name { get; }

    private readonly IList<ISagaStep> _steps;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="name">Имя саги</param>
    public SagaBase(string name = null)
    {
      Name = name;
      _steps = new List<ISagaStep>();
    }

    /// <summary>
    /// Метод для объявления очередного шага саги
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected IConfigurableSagaStep<TCont> Step(string name = null, StepType type = StepType.Simple)
    {
      var step = new SagaStep<TCont>(name, type);
      _steps.Add(step);

      return step;
    }

    /// <summary>
    /// Метод для объявления очередного асинхронного шага саги
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected IConfigurableAsyncSagaStep<TCont> AsyncStep(string name = null, StepType type = StepType.Simple)
    {
      var step = new AsyncSagaStep<TCont>(name, type);
      _steps.Add(step);

      return step;
    }

    /// <summary>
    /// Запускает выполнение саги
    /// </summary>
    /// <param name="context">Контекст выполнения саги</param>
    /// <returns>Успешность выполнения</returns>
    public async Task<bool> Execute(TCont context)
    {
      Logger.LogInfo($"Start executing saga {Name}");

      var executedSteps = new Stack<ISagaStep>();
      var isTurningStepPassed = false;
      foreach (var step in _steps)
      {
        if (await CallStepAction(step, context))
        {
          isTurningStepPassed = isTurningStepPassed || step.Type == StepType.Turning;
          executedSteps.Push(step);
          continue;
        }

        if (isTurningStepPassed)
        {
          Logger.LogWarning($"Turning point was passed, but saga {Name} failed after it!");
          return true;
        }

        await Compensate(executedSteps, context);
        return false;
      }

      return true;
    }

    private async Task Compensate(Stack<ISagaStep> executedSteps, TCont context)
    {
      if (!executedSteps.Any())
      {
        Logger.LogInfo($"There no steps to compensate in failed saga {Name}");
        return;
      }

      Logger.LogInfo($"Start saga {Name} compensation");

      do
      {
        var step = executedSteps.Pop();
        await CallStepCompensation(step, context);
      }
      while (executedSteps.Count > 0);
    }

    private async Task<bool> CallStepAction(ISagaStep step, TCont context)
    {
      switch (step)
      {
        case IExecutableAsyncSagaStep<TCont> asyncStep:
          return await asyncStep.InvokeAction(context);
        case IExecutableSagaStep<TCont> syncStep:
          return syncStep.InvokeAction(context);
        default:
          throw new ArgumentException("Wrong type of saga step for action");
      }
    }

    private async Task CallStepCompensation(ISagaStep step, TCont context)
    {
      switch (step)
      {
        case IExecutableAsyncSagaStep<TCont> asyncStep:
          await asyncStep.InvokeCompensation(context);
          break;
        case IExecutableSagaStep<TCont> syncStep:
          syncStep.InvokeCompensation(context);
          break;
        default:
          throw new ArgumentException("Wrong type of saga step for validation");
      }
    }
  }
}
