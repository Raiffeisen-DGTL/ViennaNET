using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.FluentRule.RuleValidators
{
  /// <inheritdoc />
  /// <typeparam name="T">Тип объекта для валидации</typeparam>
  public interface IRuleValidatorAsync<in T> : IRuleValidatorBase
  {
    /// <summary>
    /// Асинхронно выполняет валидацию объекта с заданным контекстом
    /// </summary>
    /// <typeparam name="TEntity">Тип дополнительного объекта для вычисления состояния валидационного сообщения</typeparam>
    /// <param name="instance">Объект с данными для валидации</param>
    /// <param name="entity">Объект с данными для формирования сообщения. Может быть отличным от объекта с данными для валидации</param>
    /// <param name="context">Контекст валидации</param>
    /// <returns>Задачу, содержащую список валидационных сообщений</returns>
    Task<IList<IRuleMessage>> ValidateAsync<TEntity>(T instance, TEntity entity, ValidationContext context);
  }
}
