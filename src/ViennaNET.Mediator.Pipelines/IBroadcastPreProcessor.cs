using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator.Pipelines
{
  /// <inheritdoc />
  /// <summary>
  /// Defined a broadcast pre-processor
  /// </summary>
  public interface IBroadcastPreProcessor : IPipelineProcessor
  {
    /// <summary>
    /// Asynchronously process method executes before calling the Handle method on your handler
    /// </summary>
    /// <param name="message">Message instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An awaitable task</returns>
    Task ProcessAsync(IMessage message, CancellationToken cancellationToken);

    /// <summary>
    /// Synchronously process method executes before calling the Handle method on your handler
    /// </summary>
    /// <param name="message">Message instance</param>
    void Process(IMessage message);
  }
}
