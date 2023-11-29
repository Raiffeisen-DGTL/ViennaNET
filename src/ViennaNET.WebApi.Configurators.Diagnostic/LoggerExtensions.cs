using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ViennaNET.Diagnostic;

namespace ViennaNET.WebApi.Configurators.Diagnostic
{
  internal static class LoggerExtensions
  {
    private static readonly Action<ILogger, string, Exception> _diagnoseCompleted;

    static LoggerExtensions()
    {
      _diagnoseCompleted = LoggerMessage.Define<string>(LogLevel.Trace,
        new EventId(1, nameof(DiagnoseCompleted)),
        "Diagnose result:{DiagnoseResult}");
    }

    public static void DiagnoseCompleted(this ILogger logger, DiagnoseResult diagnoseResult)
    {
      if (!logger.IsEnabled(LogLevel.Trace))
      {
        return;
      }

      var resultForLog = JsonSerializer.Serialize(diagnoseResult, new JsonSerializerOptions { WriteIndented = true });
      _diagnoseCompleted(logger, resultForLog, null);
    }
  }
}