using System;
using System.Runtime.Serialization;

namespace ViennaNET.Orm.Seedwork
{
  /// <summary>
  /// Исключение, возникающее при ошибках регистрации сущности
  /// </summary>
  [Serializable]
  public class EntityRegistrationException : Exception
  {
    public EntityRegistrationException()
    {
    }

    public EntityRegistrationException(string message) : base(message)
    {
    }

    public EntityRegistrationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected EntityRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
