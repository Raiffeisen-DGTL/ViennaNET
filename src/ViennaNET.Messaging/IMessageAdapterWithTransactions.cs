using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging
{
  /// <summary>
  ///   Интерфейс адаптера для работы с очередью с поддержкой транзакционности
  /// </summary>
  public interface IMessageAdapterWithTransactions : IMessageAdapter
  {
    /// <summary>
    ///   Выполнить коммит, если сессия в транзакционном режиме
    /// </summary>
    void CommitIfTransacted(BaseMessage message);

    /// <summary>
    ///   Выполнить откат, если сессия в транзакционном режиме
    /// </summary>
    void RollbackIfTransacted();
  }
}