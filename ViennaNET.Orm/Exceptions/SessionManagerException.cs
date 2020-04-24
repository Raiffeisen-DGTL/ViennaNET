using System;
using System.Runtime.Serialization;

namespace ViennaNET.Orm.Exceptions
{
  /// <summary>
  /// Исключение, возникающее при ошибках работы с сохраненными сессиями БД
  /// </summary>
  [Serializable]
  public class SessionManagerException : Exception
  {
    public SessionManagerException()
    {
    }

    public SessionManagerException(string message) : base(message)
    {
    }

    public SessionManagerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SessionManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
