using System.Threading.Tasks;

namespace ViennaNET.Sagas
{
  /// <summary>
  ///   Описывает шаг, для выполнения
  /// </summary>
  /// <typeparam name="TCont">Контекст саги</typeparam>
  public interface IExecutableAsyncSagaStep<in TCont> where TCont : class
  {
    /// <summary>
    ///   Вызывает основное действие
    /// </summary>
    /// <param name="context">Контекст саги</param>
    /// <returns>Успешность выполнения</returns>
    Task<bool> InvokeAction(TCont context);

    /// <summary>
    ///   Вызывает функцию отката основного действия
    /// </summary>
    /// <param name="context">Контекст саги</param>
    Task InvokeCompensation(TCont context);
  }
}