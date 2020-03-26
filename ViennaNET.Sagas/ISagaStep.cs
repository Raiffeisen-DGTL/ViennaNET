namespace ViennaNET.Sagas
{
  /// <summary>
  /// Маркерный интерфейс для шага саги
  /// </summary>
  public interface ISagaStep
  {
    /// <summary>
    /// Имя шага
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Тип шага
    /// </summary>
    StepType Type { get; }
  }
}
