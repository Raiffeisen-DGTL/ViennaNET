using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;

namespace ViennaNET.WebApi.Configurators.Diagnostic
{
  /// <summary>
  /// "Пустой" диагностический сервис
  /// </summary>
  public class EmptyDiagnosticImplementor : IDiagnosticImplementor
  {
    public string Key => "empty";

    public Task<IEnumerable<DiagnosticInfo>> Diagnose()
    {
      var emptyResult = new DiagnosticInfo(Key, null);

      return Task.FromResult(new[] { emptyResult }.AsEnumerable());
    }
  }
}
