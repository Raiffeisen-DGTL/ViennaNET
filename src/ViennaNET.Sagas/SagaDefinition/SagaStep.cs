using System;
using ViennaNET.Logging;
using ViennaNET.Utils;

namespace ViennaNET.Sagas.SagaDefinition
{
  /// <summary>
  ///   Класс для описания шага саги
  /// </summary>
  /// <typeparam name="TCont"></typeparam>
  public class SagaStep<TCont> : ISagaStep, IExecutableSagaStep<TCont>, IConfigurableSagaStep<TCont> where TCont : class
  {
    private Action<TCont> _action;
    private Action<TCont> _compensation;

    /// <summary>
    ///   Конструктор
    /// </summary>
    /// <param name="name">Имя шага</param>
    /// <param name="type">Тип шага</param>
    public SagaStep(string name, StepType type)
    {
      Name = name.ThrowIfNullOrEmpty(nameof(name));
      Type = type;
    }

    /// <summary>
    ///   Устанавливает основное действие
    /// </summary>
    /// <param name="action">Основное действие</param>
    /// <returns>Текущий шаг</returns>
    public IConfigurableSagaStep<TCont> WithAction(Action<TCont> action)
    {
      _action = action;
      return this;
    }

    /// <summary>
    ///   Устанавливает функцию компенсации основного действия
    /// </summary>
    /// <param name="compensation">Функция компенсации основного действия</param>
    /// <returns>Текущий шаг</returns>
    public IConfigurableSagaStep<TCont> WithCompensation(Action<TCont> compensation)
    {
      _compensation = compensation;
      return this;
    }

    /// <summary>
    ///   Вызывает основное действие
    /// </summary>
    /// <param name="context">Контекст саги</param>
    /// <returns>Успешность выполнения</returns>
    public bool InvokeAction(TCont context)
    {
      try
      {
        Logger.LogInfo($"Executing step '{Name}' action");
        _action?.Invoke(context);

        return true;
      }
      catch (AbortSagaExecutingException ex)
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
    ///   Вызывает функцию отката основного действия
    /// </summary>
    /// <param name="context">Контекст саги</param>
    public void InvokeCompensation(TCont context)
    {
      try
      {
        if (_compensation is null)
        {
          return;
        }

        Logger.LogInfo($"Executing step '{Name}' compensation");
        _compensation?.Invoke(context);
      }
      catch (Exception ex)
      {
        Logger.LogErrorFormat(ex, $"Unexpected error on executing step '{Name}' compensation");
      }
    }

    /// <summary>
    ///   Имя шага
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///   Тип шага
    /// </summary>
    public StepType Type { get; }
  }
}