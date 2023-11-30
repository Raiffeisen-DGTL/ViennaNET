using System;
using Microsoft.Extensions.Logging;
using ViennaNET.Utils;

namespace ViennaNET.Orm.Factories
{
  internal class ExplicitNhSessionScope : IDisposable
  {
    private readonly ILogger _logger;
    private bool _disposed;

    public ExplicitNhSessionScope(ISessionManager manager, ILogger<ExplicitNhSessionScope> logger)
    {
      SessionManager = manager.ThrowIfNull(nameof(manager));
      _logger = logger.ThrowIfNull(nameof(logger));
      SessionManager.UnregisterUoW();
      _disposed = false;
    }

    public ISessionManager SessionManager { get; }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    public event EventHandler Disposed;

    private void Dispose(bool disposing)
    {
      if (_disposed)
      {
        return;
      }

      if (disposing)
      {
        try
        {
          SessionManager.CloseAll();
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error to close NH Session");
        }
      }

      Disposed?.Invoke(this, EventArgs.Empty);
      _disposed = true;
    }

    ~ExplicitNhSessionScope()
    {
      Dispose(false);
    }
  }
}