using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator.Pipelines
{
  /// <inheritdoc />
  /// <summary>
  ///   Defined a message pre-processor
  /// </summary>
  /// <typeparam name="TMessage">Message type</typeparam>
  public interface IMessagePreProcessor<in TMessage> : IPipelineProcessor where TMessage : class, IMessage
  {
    /// <summary>
    ///   Asynchronously process method executes before calling the Handle method on your handler
    /// </summary>
    /// <param name="message">Message instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An awaitable task</returns>
    Task ProcessAsync(TMessage message, CancellationToken cancellationToken);

    /// <summary>
    ///   Synchronously process method executes before calling the Handle method on your handler
    /// </summary>
    /// <param name="message">Message instance</param>
    void Process(TMessage message);
  }
}