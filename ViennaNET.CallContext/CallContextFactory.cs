using System.Collections.Generic;
using ViennaNET.Utils;

namespace ViennaNET.CallContext
{
  /// <summary>
  /// Класс для извлечения актуального контекста вызова
  /// </summary>
  public class CallContextFactory : ICallContextFactory
  {
    private readonly IEnumerable<ICallContextAccessor> _accessors;

    public CallContextFactory(IEnumerable<ICallContextAccessor> accessors)
    {
      _accessors = accessors.ThrowIfNull(nameof(accessors));
    }

    /// <summary>
    /// Возвращает актуальный контекст вызова
    /// </summary>
    /// <returns></returns>
    public ICallContext Create()
    {
      foreach (var accessor in _accessors)
      {
        var context = accessor.GetContext();
        if (context != null)
        {
          return context;
        }
      }

      return new EmptyCallContext();
    }
  }
}
