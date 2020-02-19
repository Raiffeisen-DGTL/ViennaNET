namespace ViennaNET.Validation.Rules
{
  /// <summary>
  /// Базовый интерфейс валидационного правила
  /// </summary>
  /// <typeparam name="T">Тип объекта валидационного правила</typeparam>
  public interface IRuleBase<T>
  {
    /// <summary>
    /// Идентификатор правила
    /// </summary>
    RuleIdentity Identity { get; }
  }
}
