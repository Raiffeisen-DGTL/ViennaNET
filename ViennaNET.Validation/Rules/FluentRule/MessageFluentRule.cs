using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.Rules.FluentRule
{
  /// <summary>
  /// Класс для правила валидации сообщений, поддерживающего
  /// текучий интерфейс конфигурирования
  /// </summary>
  /// <typeparam name="T">Тип объекта валидации</typeparam>
  public abstract class MessageFluentRule<T> : BaseFluentRule<T>, IMessageValidation
  {
    public ValidationResult Validate(object message)
    {
      return RulesValidator.Validate(this, (T)message);
    }
  }
}
