using System.Collections.Generic;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  /// <inheritdoc />
  /// <typeparam name="T">Тип объекта для валидации</typeparam>
  public interface IRuleValidator<in T> : IRuleValidatorBase
  {
    /// <summary>
    /// Синхронно выполняет валидацию объекта с заданным контекстом
    /// </summary>
    /// <typeparam name="TEntity">Тип дополнительного объекта для вычисления состояния валидационного сообщения</typeparam>
    /// <param name="instance">Объект с данными для валидации</param>
    /// <param name="entity">Объект с данными для формирования сообщения. Может быть отличным от объекта с данными для валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Список валидационных сообщений</returns>
    IList<IRuleMessage> Validate<TEntity>(T instance, TEntity entity, ValidationContext context);
  }
}