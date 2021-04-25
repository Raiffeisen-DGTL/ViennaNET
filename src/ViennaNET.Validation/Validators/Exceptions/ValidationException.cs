using System;

namespace ViennaNET.Validation.Validators.Exceptions
{
  /// <summary>
  /// Исключение, возникающее при ошибках валидации
  /// </summary>
  public class ValidationException : Exception
  {
    /// <inheritdoc />
    public ValidationException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public ValidationException(string message, params object[] args) : base(string.Format(message, args))
    {
    }
  }
}
