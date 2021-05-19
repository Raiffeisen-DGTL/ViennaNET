using System;
using System.Runtime.Serialization;

namespace ViennaNET.Orm.Seedwork
{
  /// <summary>
  /// Исключение, возникающее при ошибках регистрации события
  /// </summary>
  [Serializable]
  public class IntegrationEventMappingRegistrationException : Exception
  {
    public IntegrationEventMappingRegistrationException()
    {
    }

    public IntegrationEventMappingRegistrationException(string message) : base(message)
    {
    }

    public IntegrationEventMappingRegistrationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected IntegrationEventMappingRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
