using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.Rules
{
  public interface IMessageValidation
  {
    ValidationResult Validate(object message);
  }
}
