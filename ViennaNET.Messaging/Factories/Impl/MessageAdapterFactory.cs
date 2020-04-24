using System;
using System.Collections.Generic;
using System.Linq;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Factories.Impl
{
  /// <inheritdoc />
  public class MessageAdapterFactory : IMessageAdapterFactory
  {
    private readonly IEnumerable<IMessageAdapterConstructor> _constructors;

    /// <summary>
    ///   Инициализирует экземпляр ссылкой на коллекцию <see cref="IMessageAdapterConstructor" />
    /// </summary>
    /// <param name="constructors"></param>
    public MessageAdapterFactory(IEnumerable<IMessageAdapterConstructor> constructors)
    {
      _constructors = constructors.ThrowIfNull(nameof(constructors));
    }

    /// <inheritdoc />
    public IMessageAdapter Create(string queueId, bool isDiagnostic)
    {
      IMessageAdapterConstructor constructor;
      try
      {
        constructor = _constructors.SingleOrDefault(x => x.HasQueue(queueId));
      }
      catch (Exception e)
      {
        throw new MessagingConfigurationException(e, $"There are too many constructors for queue: '{queueId}'");
      }

      if (constructor == null)
      {
        throw new MessagingConfigurationException($"There are no constructors for queue: '{queueId}'");
      }


      return constructor.Create(queueId, isDiagnostic);
    }
  }
}