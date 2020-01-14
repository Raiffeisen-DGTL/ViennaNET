using Company.Logging.Contracts;
using Company.Logging.Log4NetImpl;
using System;
using Microsoft.Extensions.Configuration;

namespace Company.Logging.Configuration
{
  public class LoggerJsonCfgFileConfiguration : ILogConfiguration
  {
    private const string ExceptionMessage =
      "[A]Company.Tools.Log.Configuration.LoggerConfiguration cannot be cast to [B]Company.Tools.Log.Configuration.LoggerConfiguration";
    private readonly string _sectionName;
    private readonly IConfigurationApplier _applier;
    private readonly IConfiguration _configuration;

    public LoggerJsonCfgFileConfiguration(IConfiguration configuration, string sectionName, IConfigurationApplier applier)
    {
      _sectionName = sectionName;
      _applier = applier;
      _configuration = configuration;
    }

    public LoggerJsonCfgFileConfiguration(IConfiguration configuration, string sectionName = "logger") :
      this(configuration, sectionName, new Log4NetConfigurationApplier()) {}

    public ILog BuildLogger()
    {
      LoggerConfiguration configuration;
      try
      {
        configuration = _configuration.GetSection(_sectionName)
                                      .Get<LoggerConfiguration>();
      }
      catch (InvalidCastException ex)
      {
        if (ex.Message.Contains(ExceptionMessage))
        {
          configuration = new LoggerConfiguration {IsFromConfig = true};
        }
        else
        {
          throw;
        }
      }
      if (configuration == null)
      {
        return null;
      }
      return _applier.GetLogger(configuration);
    }
  }
}