using System;
using ViennaNET.Logging.Contracts;

namespace ViennaNET.Logging
{
  public class ScopeLogRedirector: IDisposable
  {
    public ScopeLogRedirector(ICategoryLogger logger )
      :this(logger.Category)
    {
      
    }

    

    public ScopeLogRedirector(string category)
    {
        Logger.RedirectOn(category);
    }

    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_disposed)
      {
        return;
      }
      try
      {
        Logger.RedirectOff();
        _disposed = true;
      }
// ReSharper disable EmptyGeneralCatchClause
      catch {}
// ReSharper restore EmptyGeneralCatchClause
      if (disposing)
      {
        GC.SuppressFinalize(this);
      }
    }


    ~ScopeLogRedirector()
    {
      Dispose(false);
    }

    private bool _disposed;
  }
}
