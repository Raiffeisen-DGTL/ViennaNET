using System;
using System.Runtime.Serialization;

namespace ViennaNET.Orm.Exceptions
{
  /// <summary>
  /// Исключение, возникающее при ошибках регистрации маппинга NHibernate
  /// </summary>
  [Serializable]
  public class EntityMappingRegistrationException : Exception
  {
    public EntityMappingRegistrationException()
    {
    }

    public EntityMappingRegistrationException(string message) : base(message)
    {
    }

    public EntityMappingRegistrationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected EntityMappingRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
