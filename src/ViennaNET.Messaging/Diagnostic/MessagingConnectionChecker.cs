using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.Diagnostic
{
  /// <inheritdoc />
  public class MessagingConnectionChecker : IDiagnosticImplementor
  {
    private readonly IEnumerable<IMessageAdapterConstructor> _constructors;
    private readonly ILogger _logger;

    /// <summary>
    ///   Инициализирует экземпляр ссылкой на коллекцию <see cref="IMessageAdapterConstructor" />
    /// </summary>
    /// <param name="constructors">Коллекция конструкторов адаптеров</param>
    /// <param name="logger">Интерфейс логгирования</param>
    public MessagingConnectionChecker(IEnumerable<IMessageAdapterConstructor> constructors,
      ILogger<MessagingConnectionChecker> logger)
    {
      _constructors = constructors;
      _logger = logger;
    }

    /// <inheritdoc />
    public string Key => "queue_messaging";

    /// <inheritdoc />
    public Task<IEnumerable<DiagnosticInfo>> Diagnose()
    {
      return Task.FromResult(_constructors.SelectMany(CheckConstructor));
    }

    private IEnumerable<DiagnosticInfo> CheckConstructor(IMessageAdapterConstructor constructor)
    {
      try
      {
        var adapters = constructor.CreateAll()
          .Where(x => x.Configuration.IsHealthCheck)
          .ToList();
        var result = new List<DiagnosticInfo>(adapters.Count);

        foreach (var messageAdapter in adapters)
        {
          var itemConfig = messageAdapter.Configuration;
          try
          {
            messageAdapter.Connect();
            messageAdapter.Disconnect();

            _logger.LogTrace(
              "Diagnostic of messaging queue with id {queueId} has been done successfully",
              itemConfig.Id);

            result.Add(new DiagnosticInfo(itemConfig.Id, string.Empty));
          }
          catch (Exception e)
          {
            _logger.LogTrace("Diagnostic of messaging queue with id {queueId} has been failed with error: {error}",
              itemConfig.Id, e);
            result.Add(new DiagnosticInfo(itemConfig.Id, string.Empty, DiagnosticStatus.QueueError, string.Empty,
              e.ToString()));
          }
          finally
          {
            messageAdapter.Dispose();
          }
        }

        return result;
      }
      catch (Exception e)
      {
        _logger.LogTrace("Diagnostic of messaging constructor has been failed with error: {error}", e);
        return new List<DiagnosticInfo>
        {
          new("constructor", null, DiagnosticStatus.ConfigError, string.Empty, e.ToString())
        };
      }
    }
  }
}