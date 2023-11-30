using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Factories.Impl
{
  /// <inheritdoc />
  public abstract class QueueMessageAdapterConstructorBase<TConf, TQueueConf> : IMessageAdapterConstructor
    where TQueueConf : QueueConfigurationBase where TConf : ConfigurationsListBase<TQueueConf>
  {
    private readonly TConf? _configuration;

    /// <summary>
    ///   Инициализирует конструктор адаптеров экземпляром класса <see cref="IConfiguration" />
    ///   и именем секции с очередями типа конструктора в конфигурации
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    protected QueueMessageAdapterConstructorBase(IConfiguration configuration, string sectionName)
    {
      _configuration = configuration.ThrowIfNull(nameof(configuration))
        .GetSection(sectionName)
        .Get<TConf>();

      if (_configuration != null)
      {
        ValidateConfiguration();
      }
    }

    /// <inheritdoc />
    public IMessageAdapter Create(string queueId)
    {
      var queueConfiguration = _configuration?.GetQueueConfiguration(queueId);

      if (queueConfiguration == null)
      {
        throw new MessagingConfigurationException(
          $"There are no configuration with id '{queueId}' in configuration file");
      }

      return CreateAdapter(queueConfiguration);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IMessageAdapter> CreateAll()
    {
      return _configuration == null
        ? Array.Empty<IMessageAdapter>()
        : _configuration.Queues.Select(CreateAdapter).ToArray();
    }

    /// <inheritdoc />
    public bool HasQueue(string queueId)
    {
      return _configuration?.GetQueueConfiguration(queueId) != null;
    }

    private void ValidateConfiguration()
    {
      var hasErrors = false;
      var message = new StringBuilder();

      foreach (var queueConfig in _configuration.Queues)
      {
        var context = new ValidationContext(queueConfig, null);
        var results = new LinkedList<ValidationResult>();
        if (!Validator.TryValidateObject(queueConfig, context, results, true))
        {
          var errors = results.Select(r =>
            $"{string.Join(", ", r.MemberNames)} => '{r.ErrorMessage}'.");
          var allErrors = string.Join(Environment.NewLine, errors);
          message.AppendLine(
            $"Configuration for queue {queueConfig.Id} failed validation:{Environment.NewLine}{allErrors}");
          hasErrors = true;
        }
      }

      if (hasErrors)
      {
        throw new MessagingException(message.ToString());
      }
    }

    /// <summary>
    ///   Создает экземпляр адаптера в соответствии с типом конструктора
    /// </summary>
    /// <param name="queueConfiguration">Конфигурация очереди</param>
    /// <returns></returns>
    protected abstract IMessageAdapter CreateAdapter(TQueueConf queueConfiguration);
  }
}