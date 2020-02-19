using ViennaNET.Logging.Contracts;
using ViennaNET.Logging.Log4NetImpl;
using System;

namespace ViennaNET.Logging.Configuration
{
  internal sealed class FluentConfiguration : ILogConfiguration
  {
    private readonly IConfigurationApplier _applier;
    internal readonly LoggerConfiguration _configuration;

    public FluentConfiguration() : this(new Log4NetConfigurationApplier())
    {
      _configuration = new LoggerConfiguration();
    }

    public FluentConfiguration(IConfigurationApplier applier)
    {
      _applier = applier;
    }

    public ILog BuildLogger()
    {
      return _applier.GetLogger(_configuration);
    }

    public FluentConfiguration Enable()
    {
      _configuration.Enabled = true;
      return this;
    }

    public FluentConfiguration Disable()
    {
      _configuration.Enabled = false;
      return this;
    }

    public FluentConfiguration AddConsoleListener(string category = "")
    {
      _configuration.Listeners.Add(new LogListener { Category = category, Type = "console" });
      return this;
    }

    public FluentConfiguration SetMinLevel(int index, LogLevel level)
    {
      _configuration.Listeners[index]
                    .MinLevel = level;
      return this;
    }

    public FluentConfiguration SetMaxLevel(int index, LogLevel level)
    {
      _configuration.Listeners[index]
                    .MaxLevel = level;
      return this;
    }

    public FluentConfiguration AddTextListener(string category = "")
    {
      var listener = new LogListener { Category = category, Type = TextFileConstants.Type };
      _configuration.Listeners.Add(listener);
      listener.Params[TextFileConstants.Append] = TextFileConstants.AppendValue;
      listener.Params[TextFileConstants.FileName] = "application.log";
      return this;
    }

    public FluentConfiguration SetTextFileAppendMode(int index, bool isAppendMode)
    {
      AssertTextFileType(index);
      _configuration.Listeners[index]
                    .Params[TextFileConstants.Append] = isAppendMode
        ? TextFileConstants.Append
        : TextFileConstants.RolloverValue;
      return this;
    }

    public FluentConfiguration SetTextFileMaxFileSize(int index, int size)
    {
      AssertTextFileType(index);
      if (size <= 0)
      {
        throw new ArgumentException("size can not be less or equals than zero");
      }

      _configuration.Listeners[index]
                    .Params[TextFileConstants.MaxSize] = size.ToString();
      return this;
    }

    public FluentConfiguration SetTextFileMaxRollBackupsNumber(int index, int number)
    {
      AssertTextFileType(index);
      if (number <= 0)
      {
        throw new ArgumentException("Roll backups number can not be less or equals than zero");
      }

      _configuration.Listeners[index]
                    .Params[TextFileConstants.RollBackBackups] = number.ToString();
      return this;
    }

    public FluentConfiguration SetTextFileName(int index, string name)
    {
      AssertTextFileType(index);
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("File name can not be empty");
      }

      _configuration.Listeners[index]
                    .Params[TextFileConstants.FileName] = name;
      return this;
    }

    public FluentConfiguration SetTextFilePatternName(int index, string name)
    {
      AssertTextFileType(index);
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("File pattern name can not be empty");
      }

      _configuration.Listeners[index]
                    .Params.Remove(TextFileConstants.FileName);
      _configuration.Listeners[index]
                    .Params[TextFileConstants.FilePatternName] = name;
      return this;
    }

    private void AssertTextFileType(int index)
    {
      if (_configuration.Listeners[index]
                        .Type != TextFileConstants.Type)
      {
        throw new InvalidOperationException(string.Format("listener with index {0} has another type", index));
      }
    }
  }
}