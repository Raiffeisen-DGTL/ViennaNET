using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ViennaNET.Logging.Contracts;
using ViennaNET.Utils;

namespace ViennaNET.Logging
{
  /// <inheritdoc />
  [ExcludeFromCodeCoverage]
  public sealed class DefaultCategoryLogger : ICategoryLogger
  {
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for DI container
    /// </summary>
    /// <param name="logger">microsoft logger</param>
    public DefaultCategoryLogger(ILogger<DefaultCategoryLogger> logger)
    {
      _logger = logger.ThrowIfNull(nameof(logger));
    }

    /// <inheritdoc />
    public string Category => nameof(DefaultCategoryLogger);

    /// <inheritdoc />
    public void Log(LogLevel level, string message)
    {
      switch (level)
      {
        case LogLevel.Debug:
          LogDebug(message);
          break;
        case LogLevel.Info:
          LogInfo(message);
          break;
        case LogLevel.Error:
          LogError(message);
          break;
        case LogLevel.Warning:
          LogWarning(message);
          break;
        case LogLevel.Diagnostic:
          _logger.LogTrace(message);
          break;
      }
    }

    /// <inheritdoc />
    public void LogDebug(string message)
    {
      _logger.LogDebug(message);
    }

    /// <inheritdoc />
    public void LogInfo(string message)
    {
      _logger.LogInformation(message);
    }

    /// <inheritdoc />
    public void LogWarning(string message)
    {
      _logger.LogWarning(message);
    }

    /// <inheritdoc />
    public void LogError(string message)
    {
      _logger.LogError(message);
    }

    /// <inheritdoc />
    public void LogDebugFormat(string message, params object[] formatParams)
    {
      LogDebug(string.Format(message, formatParams));
    }

    /// <inheritdoc />
    public void LogInfoFormat(string message, params object[] formatParams)
    {
      LogInfo(string.Format(message, formatParams));
    }

    /// <inheritdoc />
    public void LogWarningFormat(string message, params object[] formatParams)
    {
      LogWarning(string.Format(message, formatParams));
    }

    /// <inheritdoc />
    public void LogErrorFormat(string message, params object[] formatParams)
    {
      LogError(string.Format(message, formatParams));
    }

    /// <inheritdoc />
    public void LogWarningFormat(Exception exception, string message, params object[] formatParams)
    {
      LogWarning($"{string.Format(message, formatParams)}{Environment.NewLine}{exception}");
    }

    /// <inheritdoc />
    public void LogErrorFormat(Exception exception, string message, params object[] formatParams)
    {
      LogError($"{string.Format(message, formatParams)}{Environment.NewLine}{exception}");
    }

    /// <inheritdoc />
    public void LogError(Exception exception, string message)
    {
      LogError($"{message}{Environment.NewLine}{exception}");
    }

    /// <inheritdoc />
    public void Flush()
    {
      // do nothing
    }
  }
}