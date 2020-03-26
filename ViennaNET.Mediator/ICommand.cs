using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator
{
  /// <summary>
  /// Интерфейс-маркер, определяющий команду
  /// </summary>
  public interface ICommand : IMessage
  {
    /// <summary>
    /// Признак выполнения команды
    /// </summary>
    bool IsCompleted { get; set; }

    /// <summary>
    /// Причина, по которой команда не была выполена
    /// </summary>
    object Reason { get; set; }
  }
}
