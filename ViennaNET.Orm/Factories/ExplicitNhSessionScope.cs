using System;
using ViennaNET.Logging;
using ViennaNET.Utils;

namespace ViennaNET.Orm.Factories
{
  internal class ExplicitNhSessionScope : IDisposable
  {
    private bool _disposed;

    public event EventHandler Disposed;

    public ISessionManager SessionManager { get; }

    public ExplicitNhSessionScope(ISessionManager manager)
    {
      SessionManager = manager.ThrowIfNull(nameof(manager));
      SessionManager.UnregisterUoW();
      _disposed = false;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

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
          Logger.LogErrorFormat(ex, "Error to close NH Session");
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
