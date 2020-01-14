using Company.Logging.Configuration;

namespace Company.Logging.Contracts
{
  public interface IConfigurationApplier
  {
    ILog GetLogger(LoggerConfiguration confguration);
  }
}