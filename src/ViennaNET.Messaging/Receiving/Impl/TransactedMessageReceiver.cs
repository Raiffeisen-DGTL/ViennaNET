using ViennaNET.Messaging.Tools;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Receiving.Impl
{
  /// <inheritdoc cref=""/>
  public class TransactedMessageReceiver<TMessage> : MessageReceiver<TMessage>, ITransactedMessageReceiver<TMessage>
  {
    private readonly IMessageAdapterWithTransactions _adapter;
    /// <summary>
    ///  Конструктор, инициализирующий экземпляр зависимостями
    ///  <see cref="IMessageAdapter"/> и <see cref="IMessageDeserializer"/>>
    /// </summary>
    /// <param name="adapter"></param>
    /// <param name="deserializer"></param>
    public TransactedMessageReceiver(IMessageAdapterWithTransactions adapter, IMessageDeserializer<TMessage> deserializer) : base(adapter, deserializer)
    {
      _adapter = adapter.ThrowIfNull(nameof(adapter));
    }

    /// <inheritdoc />
    public void CommitIfTransacted()
    {
      _adapter.CommitIfTransacted(null);
    }

    /// <inheritdoc />
    public void RollbackIfTransacted()
    {
      _adapter.RollbackIfTransacted();
    }
  }
}
