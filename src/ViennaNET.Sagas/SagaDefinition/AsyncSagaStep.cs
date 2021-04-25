using System;
using System.Threading.Tasks;
using ViennaNET.Logging;
using ViennaNET.Utils;

namespace ViennaNET.Sagas.SagaDefinition
{
  /// <summary>
  /// Класс для описания асинхронного шага саги
  /// </summary>
  /// <typeparam name="TCont"></typeparam>
  public class AsyncSagaStep<TCont> : ISagaStep, IExecutableAsyncSagaStep<TCont>, IConfigurableAsyncSagaStep<TCont> where TCont : class
  {
    private Func<TCont, Task> _action;
    private Func<TCont, Task> _compensation;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="name">Имя шага</param>
    /// <param name="type">Тип шага</param>
    public AsyncSagaStep(string name, StepType type)
    {
      Name = name.ThrowIfNullOrEmpty(nameof(name));
      Type = type;
    }

    /// <summary>
    /// Имя шага
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Тип шага
    /// </summary>
    public StepType Type { get; }

    /// <summary>
    /// Устанавливает основное действие
    /// </summary>
    /// <param name="action">Основное действие</param>
    /// <returns>Текущий шаг</returns>
    public IConfigurableAsyncSagaStep<TCont> WithAction(Func<TCont, Task> action)
    {
      _action = action;
      return this;
    }

    /// <summary>
    /// Устанавливает функцию компенсации основного действия
    /// </summary>
    /// <param name="compensation">Функция компенсации основного действия</param>
    /// <returns>Текущий шаг</returns>
    public IConfigurableAsyncSagaStep<TCont> WithCompensation(Func<TCont, Task> compensation)
    {
      _compensation = compensation;
      return this;
    }

    /// <summary>
    /// Вызывает основное действие
    /// </summary>
    /// <param name="context">Контекст саги</param>
    /// <returns>Успешность выполнения</returns>
    public async Task<bool> InvokeAction(TCont context)
    {
      try
      {
        if (_action is null)
        {
          return true;
        }

        Logger.LogInfo($"Executing step '{Name}' action");
        await _action.Invoke(context);

        return true;
      }
      catch (AbortSagaExecutingException)
      {
        Logger.LogInfo($"Abort saga execution by AbortSagaExecutingException on executing step '{Name}' action");
        return false;
      }
      catch (Exception ex)
      {
        Logger.LogErrorFormat(ex, $"Unexpected error on executing step '{Name}' action");
        return false;
      }
    }

    /// <summary>
    /// Вызывает функцию отката основного действия
    /// </summary>
    /// <param name="context">Контекст саги</param>
    public async Task InvokeCompensation(TCont context)
    {
      try
      {
        if (_compensation is null)
        {
          return;
        }

        Logger.LogInfo($"Executing step '{Name}' compensation");
        await _compensation.Invoke(context);
      }
      catch (Exception ex)
      {
        Logger.LogErrorFormat(ex, $"Unexpected error on executing step '{Name}' compensation");
      }
    }
  }
}
