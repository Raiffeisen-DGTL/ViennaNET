using ViennaNET.Validation.Rules.ValidationResults;

namespace ViennaNET.Validation.Rules
{
  /// <inheritdoc />
  public interface IRule<T> : IRuleBase<T>
  {
    /// <summary>
    /// Синхронно проверяет объект на соответствие правилу
    /// </summary>
    /// <param name="value">Ссылка на объект</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Результат валидации правила</returns>
    RuleValidationResult Validate(T value, ValidationContext context);
  } 
}
