using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.KafkaQueue
{
    /// <summary>
    ///   Настройки очереди
    /// </summary>
    public class KafkaQueueConfiguration : QueueConfigurationBase, IValidatableObject
    {
        /// <summary>
        ///   Определяет, является ли адаптер потребителем (True) либо отправителем (False)
        /// </summary>
        [JsonIgnore]
        public bool IsConsumer => ConsumerConfig is not null;

        /// <summary>
        ///   Имя очереди/топика
        /// </summary>
        [Required]
        public string QueueName { get; set; }

        /// <summary>
        ///   Признак работы в транзакции
        /// </summary>
        public bool TransactionEnabled { get; set; }

        /// <summary>
        ///   Полная нативная конфигурация для подписчика Kafka
        /// </summary>
        public ConsumerConfig? ConsumerConfig { get; set; }

        /// <summary>
        ///   Полная нативная конфигурация для издателя Kafka
        /// </summary>
        public ProducerConfig? ProducerConfig { get; set; }

        /// <summary>
        /// Таймаут в мс метода включения транзакций (InitTransactions)
        /// </summary>
        [Range(1, 3_600_000)]
        public int InitTransactionsTimeout { get; set; } = 1000;

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProcessingType != MessageProcessingType.ThreadStrategy)
            {
                yield return new ValidationResult("Only ThreadStrategy is supported",
                  new[] { nameof(ProcessingType) });
            }

            if (ConsumerConfig is not null && ProducerConfig is not null)
            {
                yield return new ValidationResult("Only one of ConsumerConfig and ProducerConfig can be set",
                                                  new[] { nameof(ConsumerConfig), nameof(ProducerConfig) });
            }

            if (ConsumerConfig is null && ProducerConfig is null)
            {
                yield return new ValidationResult("ConsumerConfig or ProducerConfig must be set",
                                                  new[] { nameof(ConsumerConfig), nameof(ProducerConfig) });
            }
        }
    }
}