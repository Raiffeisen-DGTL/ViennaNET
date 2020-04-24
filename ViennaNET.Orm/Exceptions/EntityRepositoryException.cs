using System;
using System.Runtime.Serialization;

namespace ViennaNET.Orm.Exceptions
{
  /// <summary>
  /// Исключение, возникающее при ошибках работы с сущностями в репозитории
  /// </summary>
  [Serializable]
  public class EntityRepositoryException : Exception
  {
    public EntityRepositoryException()
    {
    }

    public EntityRepositoryException(string message) : base(message)
    {
    }

    public EntityRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected EntityRepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
