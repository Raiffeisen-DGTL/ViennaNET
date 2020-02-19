using System.Threading.Tasks;
using ViennaNET.Validation.Rules.ValidationResults;

namespace ViennaNET.Validation.Rules
{
  /// <inheritdoc />
  public interface IRuleAsync<T> : IRuleBase<T>
  {
    /// <summary>
    /// Асинхронно проверяет объект на соответствие правилу
    /// </summary>
    /// <param name="value">Ссылка на объект</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Задача с результатом валидации правила</returns>
    Task<RuleValidationResult> ValidateAsync(T value, ValidationContext context);
  }
}
