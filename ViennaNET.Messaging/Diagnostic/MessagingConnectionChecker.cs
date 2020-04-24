using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Logging;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.Diagnostic
{
  /// <inheritdoc />
  public class MessagingConnectionChecker : IDiagnosticImplementor
  {
    private readonly IEnumerable<IMessageAdapterConstructor> _constructors;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на коллекцию <see cref="IMessageAdapterConstructor"/>
    /// </summary>
    /// <param name="constructors">Коллекция конструкторов адаптеров</param>
    public MessagingConnectionChecker(IEnumerable<IMessageAdapterConstructor> constructors)
    {
      _constructors = constructors;
    }

    /// <inheritdoc />
    public string Key => "queue_messaging";

    /// <inheritdoc />
    public Task<IEnumerable<DiagnosticInfo>> Diagnose()
    {
      return Task.FromResult(_constructors.SelectMany(CheckConstructor));
    }

    private static IEnumerable<DiagnosticInfo> CheckConstructor(IMessageAdapterConstructor constructor)
    {
      try
      {
        var adapters = constructor.CreateAll(true)
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

            Logger.LogDiagnostic($"Diagnostic of messaging queue with id {itemConfig.Id} has been done successfully");

            result.Add(new DiagnosticInfo(itemConfig.Id, itemConfig.Server));
          }
          catch (Exception e)
          {
            Logger.LogDiagnostic($"Diagnostic of messaging queue with id {itemConfig.Id} has been failed with error: {e}");
            result.Add(new DiagnosticInfo(itemConfig.Id, itemConfig.Server, DiagnosticStatus.QueueError, string.Empty, e.ToString()));
          }
        }

        return result;
      }
      catch (Exception e)
      {
        Logger.LogDiagnostic($"Diagnostic of messaging constructor has been failed with error: {e}");
        return new List<DiagnosticInfo>
        {
          new DiagnosticInfo("constructor", null, DiagnosticStatus.ConfigError, string.Empty, e.ToString())
        };
      }
    }
  }
}