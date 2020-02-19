using System.Collections.Generic;
using System.Linq;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Validators
{
  /// <summary>
  /// Результат валидации
  /// </summary>
  public sealed class ValidationResult
  {
    /// <summary>
    /// Инициализирует экземпляр пустой коллекцией результатов правил
    /// </summary>
    public ValidationResult()
    {
      Results = new List<RuleValidationResult>();
    }

    /// <summary>
    /// Инициализирует экземпляр результатом правила
    /// </summary>
    public ValidationResult(RuleValidationResult result) : this()
    {
      if (result != null)
      {
        Results.Add(result);
      }
    }

    /// <summary>
    /// Признак отсутствия ошибки
    /// </summary>
    public bool IsValid
    {
      get { return Results.All(x => x.IsValid); }
    }

    /// <summary>
    /// Результаты правил
    /// </summary>
    public List<RuleValidationResult> Results { get; }

    /// <summary>
    /// Получает результат валидации по индексу
    /// </summary>
    /// <param name="index">Индекс коллекции</param>
    /// <returns>Сообщения правила валидации</returns>
    public IEnumerable<IRuleMessage> GetMessagesByResultIndex(int index)
    {
      return Results[index]
        .Messages;
    }

    /// <summary>
    /// Соединяет два результата валидации
    /// </summary>
    /// <param name="result">Результат валидации</param>
    public void MergeResult(ValidationResult result)
    {
      Results.AddRange(result.Results);
    }
  }
}
