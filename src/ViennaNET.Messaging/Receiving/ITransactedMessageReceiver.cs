namespace ViennaNET.Messaging.Receiving
{
  /// <summary>
  ///   Позволяет подтвердить обработку сообщения в случае транзакционности источника
  /// </summary>
  public interface ITransactedMessageReceiver<TMessage> : IMessageReceiver<TMessage>
  {
    /// <summary>
    ///   Подтвердить транзакцию
    /// </summary>
    void CommitIfTransacted();

    /// <summary>
    ///   Откатить транзакцию
    /// </summary>
    void RollbackIfTransacted();
  }
}