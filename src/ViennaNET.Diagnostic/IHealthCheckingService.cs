using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Diagnostic.Data;

namespace ViennaNET.Diagnostic
{
  /// <summary>
  /// Делегат для события о непрохождении диагностики
  /// </summary>
  public delegate void DiagnosticFailedDelegate();

  /// <summary>
  /// Делегат для события об успешном прохождении диагностики
  /// </summary>
  public delegate void DiagnosticPassedDelegate();

  /// <summary>
  /// Сервис для централизованного вызова диагностики приложения
  /// </summary>
  public interface IHealthCheckingService
  {
    /// <summary>
    /// Выполняет вызов всех зарегистрированных диагностик
    /// </summary>
    /// <returns>результат диагностики</returns>
    Task<IEnumerable<DiagnosticInfo>> CheckHealthAsync();

    /// <summary>
    /// Событие о непрохождении диагностики
    /// </summary>
    event DiagnosticFailedDelegate DiagnosticFailedEvent;

    /// <summary>
    /// Событие об успешном прохождении диагностики
    /// </summary>
    event DiagnosticPassedDelegate DiagnosticPassedEvent;
  }
}
