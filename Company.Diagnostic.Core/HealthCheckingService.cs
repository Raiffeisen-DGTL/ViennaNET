using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Company.Diagnostic.Core.Data;
using Company.Utils;

namespace Company.Diagnostic.Core
{
  /// <summary>
  /// Сервис для централизованного вызова диагностики приложения
  /// </summary>
  public class HealthCheckingService : IHealthCheckingService
  {
    private readonly IEnumerable<IDiagnosticImplementor> _implementors;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="implementors">набор диагностик</param>
    public HealthCheckingService(IEnumerable<IDiagnosticImplementor> implementors)
    {
      _implementors = implementors.ThrowIfNull(nameof(implementors));
    }

    /// <summary>
    /// Выполняет вызов всех зарегистрированных диагностик
    /// </summary>
    /// <returns>результат диагностики</returns>
    public async Task<IEnumerable<DiagnosticInfo>> CheckHealthAsync()
    {
      var tasks = _implementors.Select(implementor => implementor.Diagnose())
                               .ToList();
      var result = (await Task.WhenAll(tasks)
                              .ConfigureAwait(false)).SelectMany(diagnosticInfos => diagnosticInfos)
                                                     .ToList();
      if (result.Any(x => x.Status != DiagnosticStatus.Ok))
      {
        DiagnosticFailedEvent?.Invoke();
      }
      else
      {
        DiagnosticPassedEvent?.Invoke();
      }

      return result;
    }

    /// <summary>
    /// Событие о непрохождении диагностики
    /// </summary>
    public event DiagnosticFailedDelegate DiagnosticFailedEvent;

    /// <summary>
    /// Событие об успешном прохождении диагностики
    /// </summary>
    public event DiagnosticPassedDelegate DiagnosticPassedEvent;
  }
}
