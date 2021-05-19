using ViennaNET.Logging.Configuration;

namespace ViennaNET.Logging.Contracts
{
  public interface IConfigurationApplier
  {
    ILog GetLogger(LoggerConfiguration confguration);
  }
}