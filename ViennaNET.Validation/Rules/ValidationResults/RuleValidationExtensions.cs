using System;
using System.Collections.Generic;
using System.Linq;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.ValidationResults
{
  /// <summary>
  /// Методы расширения для задач форматирования валидационных результатов
  /// </summary>
  public static class RuleValidationExtensions
  {
    /// <summary>
    /// Преобразует сообщение в результат валидации, содержащим
    /// один результат выполнения правила с одной ошибкой
    /// </summary>
    /// <param name="message">Строка сообщения об ошибке</param>
    /// <returns>Результат валидации</returns>
    public static RuleValidationResult ToRuleValidationResultWithError(this string message)
    {
      return new RuleValidationResult(new RuleIdentity(Guid.NewGuid()
                                                           .ToString()), new ErrorRuleMessage(new MessageIdentity(Guid.NewGuid()
                                                                                                                      .ToString()),
                                                                                              message));
    }

    /// <summary>
    /// Преобразует сообщение в результат валидации, содержащим
    /// один результат выполнения правила с одним предупреждением
    /// </summary>
    /// <param name="message">Строка сообщения об ошибке</param>
    /// <returns>Результат валидации</returns>
    public static RuleValidationResult ToRuleValidationResultWithWarning(this string message)
    {
      return new RuleValidationResult(new RuleIdentity(Guid.NewGuid()
                                                           .ToString()), new WarningRuleMessage(new MessageIdentity(Guid.NewGuid()
                                                                                                                        .ToString()),
                                                                                                message));
    }

    /// <summary>
    /// Преобразует коллекцию результатов валидации правил в массив строк
    /// </summary>
    /// <param name="validationResults">Коллекция результатов валидации правил</param>
    /// <returns>Массив строк из результатов</returns>
    public static List<string> ToErrorsList(this IEnumerable<RuleValidationResult> validationResults)
    {
      return validationResults.SelectMany(x => x.Messages.Select(m => m.Error))
                              .ToList();
    }

    /// <summary>
    /// Преобразует коллекцию результатов валидации правил в строку,
    /// разделенную заданным разделителем
    /// </summary>
    /// <param name="validationResults">Коллекция результатов валидации правил</param>
    /// <param name="delimiter">Разделитель</param>
    /// <returns>Строка с разделителями</returns>
    public static string ToErrorsString(this IEnumerable<RuleValidationResult> validationResults, string delimiter = null)
    {
      return string.Join(delimiter ?? Environment.NewLine, validationResults.ToErrorsList());
    }

    /// <summary>
    /// Преобразует результат валидации в строку,
    /// разделенную заданным разделителем
    /// </summary>
    /// <param name="validationResult">Результат валидации</param>
    /// <param name="delimiter">Разделитель</param>
    /// <returns>Строка с разделителями</returns>
    public static string ToErrorsString(this RuleValidationResult validationResult, string delimiter = null)
    {
      return string.Join(delimiter ?? Environment.NewLine, validationResult.Messages.Select(m => m.Error));
    }
  }
}
