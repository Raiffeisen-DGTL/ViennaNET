using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator
{
  /// <summary>
  ///   Интерфейс-маркер, определяющий команду
  /// </summary>
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  public interface ICommand : IMessage
  {
    /// <summary>
    ///   Признак выполнения команды
    /// </summary>
    bool IsCompleted { get; set; }

    /// <summary>
    ///   Причина, по которой команда не была выполена
    /// </summary>
    object Reason { get; set; }
  }
}