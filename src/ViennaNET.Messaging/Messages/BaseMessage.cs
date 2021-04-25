using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Messages
{
  /// <summary>
  ///   Сообщение
  /// </summary>
  [Serializable]
  public abstract class BaseMessage
  {
    /// <summary>
    ///   Конструктор по умолчанию
    /// </summary>
    protected BaseMessage()
    {
      LifeTime = TimeSpan.Zero;
      Properties = new Dictionary<string, object>();
    }

    /// <summary>
    ///   Текущий идентификатор сообщения
    /// </summary>
    [NotNull]
    public string MessageId { get; set; }

    /// <summary>
    ///   Идентификатор корреляции
    /// </summary>
    [CanBeNull]
    public string CorrelationId { get; set; }

    /// <summary>
    ///   Название приложения, которое отправляет сообщение
    /// </summary>
    [CanBeNull]
    public string ApplicationTitle { get; set; }

    /// <summary>
    ///   Дата и время отправки сообщения
    /// </summary>
    public DateTime? SendDateTime { get; set; }

    /// <summary>
    ///   Дата и время получения сообщения
    /// </summary>
    public DateTime? ReceiveDate { get; set; }

    /// <summary>
    ///   Очередь ответов
    /// </summary>
    [CanBeNull]
    public string ReplyQueue { get; set; }

    /// <summary>
    ///   Время жизни сообщения
    /// </summary>
    public TimeSpan LifeTime { get; set; }

    /// <summary>
    ///   Свойства сообщения
    /// </summary>
    [NotNull]
    [XmlIgnore]
    public Dictionary<string, object> Properties { get; }

    /// <inheritdoc />
    public override string ToString()
    {
      var serializer = new XmlSerializer(GetType());

      using var writer = new StringWriter();
      serializer.Serialize(writer, this);
      return writer.ToString();
    }

    /// <summary>
    ///   Выдает тело сообщения в виде строки для логирования
    /// </summary>
    public abstract string LogBody();

    /// <summary>
    ///   Проверяет, пустое ли тело сообщения
    /// </summary>
    /// <returns>Признак пустого сообщения</returns>
    public abstract bool IsEmpty();
  }
}