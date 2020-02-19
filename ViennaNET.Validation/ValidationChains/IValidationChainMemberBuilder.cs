using ViennaNET.Validation.Rules;

namespace ViennaNET.Validation.ValidationChains
{
  /// <summary>
  /// Интерфейс строителя, задающего поведение цепи участников валидации
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IValidationChainMemberBuilder<T>
  {
    /// <summary>
    /// Задает зависимость выполнения участника цепи от результата правила
    /// </summary>
    /// <param name="rule">Правило, определяющее выполнение участника цепи</param>
    IValidationChainMemberBuilder<T> DependsOn(IRuleBase<T> rule);

    /// <summary>
    /// Определяет, что участник цепи останавливает выполнение цепи в случае ошибки в процессе выполнения
    /// </summary>
    IValidationChainMemberBuilder<T> StopOnFailure();
  }
}
