using System.ComponentModel.DataAnnotations;

namespace ViennaNET.Messaging.Configuration
{
    /// <summary>
    ///   Базовые настройки очереди
    /// </summary>
    public abstract class QueueConfigurationBase
    {
        /// <summary>
        ///   Идентификатор
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        ///   Тип взаимодействия
        /// </summary>
        [Required]
        public MessageProcessingType ProcessingType { get; set; }

        /// <summary>
        ///   Очередь для ответа
        /// </summary>
        public string? ReplyQueue { get; set; }

        /// <summary>
        ///   Время жизни сообщения
        /// </summary>
        public TimeSpan? Lifetime { get; set; }

        /// <summary>
        ///   Пользовательские заголовки
        /// </summary>
        public List<CustomHeader>? CustomHeaders { get; set; }

        /// <summary>
        ///   Интервал для опроса очереди (в режиме ThreadStrategy - интервал опроса, в остальных режимах - интервал
        ///   переподключения), мс
        /// </summary>
        [Range(-1, 3_600_000)]
        public int IntervalPollingQueue { get; set; }

        /// <summary>
        ///   Признак зависимости от 'Health Check'
        /// </summary>
        public bool ServiceHealthDependent { get; set; }

        /// <summary>
        ///   Признак использования для 'Health Check'
        /// </summary>
        public bool IsHealthCheck { get; set; }
    }
}