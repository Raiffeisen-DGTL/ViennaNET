using System;
using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Messaging.Processing
{
  /// <summary>
  ///  Описывает возможности, которые представлены для процесса прослушивания очередей за счет потока
  /// </summary>
  public interface IPolling
  {
    /// <summary>
    /// Статус запуска
    /// </summary>
    bool IsStarted { get; }

    /// <summary>
    /// Запуск процесса прослушивания очередей
    /// </summary>
    /// <param name="processAction"><see cref="Action"/>, осуществляющий непосредственную работу с очередью.</param>
    void StartPolling(Func<CancellationToken, Task> processAction);

    /// <summary>
    /// Остановка процесса прослушивания очередей
    /// </summary>
    void StopPolling();
  }
}
