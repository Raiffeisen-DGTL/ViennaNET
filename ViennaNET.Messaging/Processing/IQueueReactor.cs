using System;

namespace ViennaNET.Messaging.Processing
{
  /// <summary>
  /// Предоставляет функционал для прослушки очередей
  /// </summary>
  public interface IQueueReactor: IDisposable
  {
    /// <summary>
    /// Количество ошибок
    /// </summary>
    int ErrorCount { get; }

    /// <summary>
    /// Запускает процесс прослушивания
    /// </summary>
    /// <Return>True - Если успешно запущен, False - если уже запущен</Return>
    bool StartProcessing();

    /// <summary>
    /// Останавливает процесс прослушивания
    /// </summary>
    void Stop();

    /// <summary>
    /// Событие заново создающие адаптер при возникновении ошибок
    /// </summary>
    event EventHandler<EventArgs> NeedReconnect;
  }
}
