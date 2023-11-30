using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator
{
  /// <summary>
  ///   Интерфейс-маркер, определяющий запрос
  /// </summary>
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  public interface IRequest : IMessage
  {}
}