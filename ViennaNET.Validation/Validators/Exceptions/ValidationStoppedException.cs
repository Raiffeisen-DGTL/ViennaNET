using System;
using ViennaNET.Validation.ValidationChains;

namespace ViennaNET.Validation.Validators.Exceptions
{
  /// <summary>
  /// Исключение, необходимое для работы функции StopOnFailure <see cref="IValidationChainMemberBuilder{T}"/>
  /// </summary>
  internal class ValidationStoppedException : Exception
  {
    public ValidationStoppedException(ValidationResult result) : base("Валидация прервана по результатам выполнения правила")
    {
      Result = result;
    }

    public ValidationResult Result { get; }
  }
}
