namespace ViennaNET.Sagas
{
  /// <summary>
  /// Описывает шаг, для выполнения
  /// </summary>
  /// <typeparam name="TCont">Контекст саги</typeparam>
  public interface IExecutableSagaStep<in TCont> where TCont : class
  {
    /// <summary>
    /// Вызывает основное действие
    /// </summary>
    /// <param name="context">Контекст саги</param>
    /// <returns>Успешность выполнения</returns>
    bool InvokeAction(TCont context);

    /// <summary>
    /// Вызывает функцию отката основного действия
    /// </summary>
    /// <param name="context">Контекст саги</param>
    void InvokeCompensation(TCont context);
  }
}
