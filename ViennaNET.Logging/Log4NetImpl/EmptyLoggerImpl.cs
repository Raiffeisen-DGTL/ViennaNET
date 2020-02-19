using ViennaNET.Logging.Contracts;

namespace ViennaNET.Logging.Log4NetImpl
{
  internal class EmptyLoggerImpl: ILog
  {
    public void Log(string category, LogLevel level, string message)
    {
      // do nothing - in case of logger initialization is failed
    }

    public void Merge(ILog logger)
    {
      // do nothing - in case of logger initialization is failed
    }

    public void Flush()
    {
      // do nothing - in case of logger initialization is failed
    }
  }
}
