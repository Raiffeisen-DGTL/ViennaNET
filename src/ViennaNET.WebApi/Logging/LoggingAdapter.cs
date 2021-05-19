using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ViennaNET.Logging;
using ViennaNET.Logging.Contracts;
using ViennaNET.Utils;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ViennaNET.WebApi.Logging
{
  /// <summary>
  /// Реализация журнала, поддерживающая обратную совместимость системы журналирования с <see cref="ViennaNET.Logging.Logger"/>,
  /// на время перехода на систему журналирования платформы <see cref="ILogger{TCategoryName}"/>.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal sealed class LoggingAdapter : ILogger, IDisposable
  {
    private ICategoryLogger _categoryLogger;
    private void Log(LogLevel logLevel, string message, Exception exception = null)
    {
      message.ThrowIfNull(nameof(message));

      switch (logLevel)
      {
        case LogLevel.Debug:
          Logger.LogDebug(message);
          break;
        case LogLevel.Information:
          Logger.LogInfo(message);
          break;
        case LogLevel.Error when exception is null:
          Logger.LogError(message);
          break;
        case LogLevel.Error:
          Logger.LogError(exception, message);
          break;
        case LogLevel.Critical when exception is null:
          Logger.LogError(message);
          break;
        case LogLevel.Critical:
          Logger.LogError(exception, message);
          break;
        case LogLevel.Warning:
          Logger.LogWarning(message);
          break;
        case LogLevel.Trace:
          Logger.LogDiagnostic(message);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
      }
    }

    private void LogCategory(LogLevel logLevel, string message, Exception exception = null)
    {
      message.ThrowIfNull(nameof(message));

      switch (logLevel)
      {
        case LogLevel.Debug:
          _categoryLogger.LogDebug(message);
          break;
        case LogLevel.Information:
          _categoryLogger.LogInfo(message);
          break;
        case LogLevel.Error when exception is null:
          _categoryLogger.LogError(message);
          break;
        case LogLevel.Error:
          _categoryLogger.LogError(exception, message);
          break;
        case LogLevel.Critical when exception is null:
          _categoryLogger.LogError(message);
          break;
        case LogLevel.Critical:
          _categoryLogger.LogError(exception, message);
          break;
        case LogLevel.Warning:
          _categoryLogger.LogWarning(message);
          break;
        case LogLevel.Trace:
          _categoryLogger.Log(ViennaNET.Logging.LogLevel.Diagnostic, message);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
      }
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
      Func<TState, Exception, string> formatter)
    {
      if (!IsEnabled(logLevel))
      {
        return;
      }

      if (_categoryLogger is null)
      {
        Log(logLevel, state.ToString(), exception);
      }
      else
      {
        LogCategory(logLevel, state.ToString(), exception);
      }
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state)
    {
      _categoryLogger = Logger.GetCategoryLogger(state.ToString());
      return this;
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
      return logLevel != LogLevel.None;
    }

    /// <inheritdoc />
    public void Dispose()
    {
      _categoryLogger = null;
    }
  }


  /// <inheritdoc />
  [ProviderAlias("ViennaNET.Logging.Logger")]
  [ExcludeFromCodeCoverage]
  internal sealed class LoggingAdapterProvider : ILoggerProvider
  {
    private readonly ConcurrentDictionary<string, LoggingAdapter> _loggers = new ConcurrentDictionary<string, LoggingAdapter>();

    public void Dispose()
    {
      _loggers.Clear();
    }

    public ILogger CreateLogger(string categoryName)
    {
      return _loggers.GetOrAdd(categoryName, new LoggingAdapter());
    }
  }
}