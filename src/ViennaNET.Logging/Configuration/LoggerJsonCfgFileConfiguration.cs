﻿using System;
using Microsoft.Extensions.Configuration;
using ViennaNET.Logging.Contracts;
using ViennaNET.Logging.Log4NetImpl;

namespace ViennaNET.Logging.Configuration
{
  public class LoggerJsonCfgFileConfiguration : ILogConfiguration
  {
    private const string ExceptionMessage =
      "[A]ViennaNET.Tools.Log.Configuration.LoggerConfiguration cannot be cast to [B]ViennaNET.Tools.Log.Configuration.LoggerConfiguration";

    private readonly IConfigurationApplier _applier;
    private readonly IConfiguration _configuration;
    private readonly string _sectionName;

    public LoggerJsonCfgFileConfiguration(IConfiguration configuration, string sectionName,
      IConfigurationApplier applier)
    {
      _sectionName = sectionName;
      _applier = applier;
      _configuration = configuration;
    }

    public LoggerJsonCfgFileConfiguration(IConfiguration configuration, string sectionName = "logger") :
      this(configuration, sectionName, new Log4NetConfigurationApplier())
    {
    }

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
          configuration = new LoggerConfiguration { IsFromConfig = true };
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