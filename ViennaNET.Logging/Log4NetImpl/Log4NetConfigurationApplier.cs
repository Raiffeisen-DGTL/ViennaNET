#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ViennaNET.Logging.Configuration;
using ViennaNET.Logging.Contracts;
using log4net;
using log4net.Appender;
using log4net;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using ILog = ViennaNET.Logging.Contracts.ILog;
using log4net.Core;

#endregion

namespace ViennaNET.Logging.Log4NetImpl
{
  internal class Log4NetConfigurationApplier : IConfigurationApplier
  {
    private const string AppDomainKey = "LoggerInit";
    private const string RepositoryName = "Repository";
    private const string DeafultPatternLayout = "%date [%property{log4net:HostName}][%property{service}][%thread][%property{user}][%property{request_id}] %-5level - %message%newline";
    private readonly Dictionary<LogLevel, Level> _levelMap = new Dictionary<LogLevel, Level>();

    public Log4NetConfigurationApplier()
    {
      _levelMap.Add(LogLevel.Debug, Level.Debug);
      _levelMap.Add(LogLevel.Info, Level.Info);
      _levelMap.Add(LogLevel.Warning, Level.Warn);
      _levelMap.Add(LogLevel.Error, Level.Error);
    }

    private Dictionary<string, log4net.ILog> CreateCategoryLoggers(LoggerConfiguration confguration)
    {
      if (!LogManager.GetAllRepositories().Any(x => x.Name == RepositoryName))
      {
        LogManager.CreateRepository(RepositoryName);
      }

      var hierarchy = (Hierarchy)LogManager.GetRepository(RepositoryName);
      var categoryLoggers = new Dictionary<string, log4net.Repository.Hierarchy.Logger>();

      foreach (string category in confguration.Listeners.Select(l => l.Category).Distinct())
      {
        string translatedCategory = category == "All" ? string.Empty : category;
        var log = (log4net.Repository.Hierarchy.Logger)hierarchy.GetLogger(translatedCategory);
        categoryLoggers.Add(translatedCategory, log);
      }

      if (confguration.Enabled && !(AlreadyDoneFromConfig && confguration.IsFromConfig))
      {
        foreach (var listener in confguration.Listeners)
        {
          var appender = GetAppender(listener, confguration);
          if (listener.Category == "All")
          {
            categoryLoggers[string.Empty].AddAppender(appender);
          }
          else
          {
            categoryLoggers[listener.Category].AddAppender(appender);
          }
        }
      }
      hierarchy.Root.Level = Level.All;
      hierarchy.Configured = true;
      var logs = categoryLoggers.Keys.ToDictionary(n => n, n => LogManager.GetLogger(RepositoryName, n));
      return logs;
    }

    private Dictionary<string, log4net.ILog> LoadFromCache()
    {
      return (Dictionary<string, log4net.ILog>)AppDomain.CurrentDomain.GetData(AppDomainKey);
    }

    public ILog GetLogger(LoggerConfiguration confguration)
    {
      if (confguration == null)
      {
        throw new ArgumentNullException("confguration");
      }
      Dictionary<string, log4net.ILog> logs;
      if (confguration.IsFromConfig && AlreadyDoneFromConfig)
      {
        logs = LoadFromCache();
      }
      else
      {
        logs = CreateCategoryLoggers(confguration);
        StoreInCache(logs);
      }
      return new Log4NetWrapper(logs);
    }

    private IAppender GetAppender(LogListener listener, LoggerConfiguration confguration)
    {
      switch (listener.Type)
      {
        case "textFile":
          return GetFileAppender(listener);
        case "console":
          return GetConsoleAppender(listener);
        default:
          return GetCustomAppender(listener, confguration);
      }
    }

    private IAppender GetCustomAppender(LogListener listener, LoggerConfiguration confguration)
    {
      var customListener = confguration.GetCustomListener(listener.Type);
      if (customListener != null)
      {
        var appender = new CustomAppender(customListener);
        var patternLayout = new PatternLayout { ConversionPattern = DeafultPatternLayout };
        patternLayout.ActivateOptions();
        appender.Layout = patternLayout;
        appender.AddFilter(new LevelRangeFilter { LevelMin = _levelMap[listener.MinLevel], LevelMax = _levelMap[listener.MaxLevel] });
        return appender;
      }
      return null;
    }

    private IAppender GetFileAppender(LogListener listener)
    {
      var appender = new CustomRollingFileAppender();
      if (listener.Params.ContainsKey(TextFileConstants.Append))
      {
        appender.RollingStyle = listener.Params[TextFileConstants.Append] == TextFileConstants.RolloverValue ||
                                listener.Params.ContainsKey(TextFileConstants.FilePatternName)
                                  ? RollingFileAppender.RollingMode.Composite
                                  : RollingFileAppender.RollingMode.Size;
      }
      else
      {
        appender.RollingStyle = RollingFileAppender.RollingMode.Size; // append default value
      }
      var patternLayout = new PatternLayout { ConversionPattern = DeafultPatternLayout };
      patternLayout.ActivateOptions();
      appender.Layout = patternLayout;
      appender.LockingModel = new FileAppender.MinimalLock();
      appender.AppendToFile = false;
      appender.AddFilter(new LevelRangeFilter { LevelMin = _levelMap[listener.MinLevel], LevelMax = _levelMap[listener.MaxLevel] });
      if (listener.Params.ContainsKey(TextFileConstants.FileName))
      {
        appender.File = listener.Params[TextFileConstants.FileName];
      }
      else
      {
        appender.StaticLogFileName = false;
        //        appender.CountDirection = 1;
        var filePattern = Path.GetFileNameWithoutExtension(listener.Params[TextFileConstants.FilePatternName]);
        var extention = Path.GetExtension(listener.Params[TextFileConstants.FilePatternName]);
        var fileNamePrefix = string.Empty;
        if (!string.IsNullOrEmpty(filePattern))
        {
          if (filePattern.Contains("'"))
          {
            int index = filePattern.IndexOf('\'', 1);
            if (index > 2 && filePattern.Length > 3)
            {
              fileNamePrefix = filePattern.Substring(1, index - 1);
              filePattern = filePattern.Substring(index + 1);
            }
          }
          appender.DatePattern = string.Format("{0}'{1}'", filePattern, extention);
        }
        var path = Path.GetDirectoryName(listener.Params[TextFileConstants.FilePatternName]);
        if (!string.IsNullOrEmpty(path))
        {
          path = path + "\\";
        }
        path = path + fileNamePrefix;
        appender.File = string.IsNullOrEmpty(path) ? "r" : path;
      }
      if (listener.Params.ContainsKey(TextFileConstants.MaxSize))
      {
        appender.MaximumFileSize = listener.Params[TextFileConstants.MaxSize] + "KB";
      }
      if (listener.Params.ContainsKey(TextFileConstants.RollBackBackups))
      {
        appender.MaxSizeRollBackups = int.Parse(listener.Params[TextFileConstants.RollBackBackups]);
      }
      if (listener.Params.ContainsKey(TextFileConstants.CountDirection))
      {
        appender.CountDirection = int.Parse(listener.Params[TextFileConstants.CountDirection]);
      }
      if (listener.Params.ContainsKey(TextFileConstants.PreserveLogFileNameExtension))
      {
        appender.PreserveLogFileNameExtension = listener.Params[TextFileConstants.PreserveLogFileNameExtension].Equals("true",
          StringComparison.InvariantCultureIgnoreCase);
      }

      appender.ActivateOptions();
      return appender;
    }

    private IAppender GetConsoleAppender(LogListener listener)
    {
      var appender = new ConsoleAppender();
      var patternLayout = new PatternLayout { ConversionPattern = DeafultPatternLayout };
      patternLayout.ActivateOptions();
      appender.Layout = patternLayout;
      appender.AddFilter(new LevelRangeFilter { LevelMin = _levelMap[listener.MinLevel], LevelMax = _levelMap[listener.MaxLevel] });
      return appender;
    }

    private static bool AlreadyDoneFromConfig
    {
      get
      {
        var obj = AppDomain.CurrentDomain.GetData(AppDomainKey);
        return obj != null;
      }
    }

    private void StoreInCache(Dictionary<string, log4net.ILog> map)
    {
      AppDomain.CurrentDomain.SetData(AppDomainKey, map);
    }
  }
}