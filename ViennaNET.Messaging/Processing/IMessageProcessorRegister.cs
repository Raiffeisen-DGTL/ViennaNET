using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Processing
{
  /// <summary>
  /// Предоставляет функционал по регистрации обработчика <see cref="IMessageProcessor"/> сообщений <see cref="BaseMessage"/>
  /// </summary>
  public interface IMessageProcessorRegister
  {
    /// <summary>
    /// Регистрация обработчика <see cref="IMessageProcessor"/> сообщений <see cref="BaseMessage" />
    /// </summary>
    /// <returns>Регистратор <see cref="IMessageProcessorRegister"/></returns>
    IMessageProcessorRegister Register<T>(string queueId) where T : class;
  }
}
