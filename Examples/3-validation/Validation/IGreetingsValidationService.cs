using ViennaNET.Validation.Validators;

namespace ValidationService.Validation
{
  public interface IGreetingsValidationService
  {
    ValidationResult ValidateGreeting(string greeting);
  }
}
