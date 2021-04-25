using System;
using System.Runtime.Serialization;

namespace ViennaNET.Orm.Exceptions
{
  /// <summary>
  /// Исключение, возникающее при ошибках в единице работы
  /// </summary>
  [Serializable]
  public class UowException : Exception
  {
    public UowException()
    {
    }

    public UowException(string message) : base(message)
    {
    }

    public UowException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected UowException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
