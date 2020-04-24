using System;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm
{
  /// <summary>
  /// Базовый класс для интеграционных событий
  /// </summary>
  public abstract class IntegrationEvent : IIntegrationEvent
  {
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public virtual int Id { get; protected set; }
    
    /// <summary>
    /// Идентификатор типа события
    /// </summary>
    public virtual int Type { get; protected set; }
    
    /// <summary>
    /// Дата стоздания
    /// </summary>
    public virtual DateTime Timestamp { get; protected set; }
    
    /// <summary>
    /// Инициатор события
    /// </summary>
    public virtual string Initiator { get; protected set; }
    
    /// <summary>
    /// Сериализованное тело события
    /// </summary>
    public virtual string Body { get; protected set; }
    
    /// <summary>
    /// Признак для отправки события в очередь при сохранении
    /// </summary>
    public virtual bool IsSendable { get; protected set; }
  }
}
