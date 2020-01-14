using Company.Diagnostic.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Company.Diagnostic.Core
{
  /// <summary>
  /// Позволяет провести диагностику компонентов. Предполагает множество
  /// импелемнтаций для получения полной картины состояния сервиса
  /// </summary>
  public interface IDiagnosticImplementor
  {
    /// <summary>
    /// Ключ конкретной реализации. Однозначно определяет текущую реализацию
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Проверяет состояние функции сервиса
    /// </summary>
    /// <returns>Результат проверки состояния</returns>
    Task<IEnumerable<DiagnosticInfo>> Diagnose();
  }
}
