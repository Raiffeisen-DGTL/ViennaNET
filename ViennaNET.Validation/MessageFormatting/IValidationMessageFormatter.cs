using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.MessageFormatting
{
  /// <summary>
  /// Предназначен для создания преобразователя сообщений валидации.
  /// Позволяет создавать правила и выполнять форматирование по ним
  /// </summary>
  public interface IValidationMessageFormatter
  {
    /// <summary>
    /// Возвращает результат валидации с отформатированными сообщениями
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    ValidationResult Format(ValidationResult result);
  }
}
