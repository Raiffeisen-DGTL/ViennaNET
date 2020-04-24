using System;
using System.Runtime.Serialization;

namespace ViennaNET.Orm.Exceptions
{
  /// <summary>
  /// Исключение, возникающее при ошибках загрузки профайдера фабрики сессий
  /// </summary>
  [Serializable]
  public class SessionFactoryProviderException : Exception
  {
    public SessionFactoryProviderException()
    {
    }

    public SessionFactoryProviderException(string message) : base(message)
    {
    }

    public SessionFactoryProviderException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SessionFactoryProviderException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
