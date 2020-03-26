using System;
using System.Runtime.Serialization;

namespace ViennaNET.Mediator.Exceptions
{
  /// <summary>
  /// Исключение в процессе выполнения команды
  /// </summary>
  [Serializable]
  public class ExecuteCommandException : Exception
  {
    /// <inheritdoc />
    public ExecuteCommandException(string message) 
      : base(message)
    {
    }

    protected ExecuteCommandException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
