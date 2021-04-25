using System;
using System.Threading.Tasks;

namespace ViennaNET.Utils.Threading
{
  /// <summary>
  ///   Асинхронная стратегия, порождающая ограниченное число потоков
  /// </summary>
  public class LimitedConcurrencyLevelProcessingStrategy : IAsyncProcessingStrategy
  {
    private readonly TaskFactory _taskFactory;

    /// <summary>
    ///   Создает объект с указанием максимального числа параллельных потоков
    /// </summary>
    /// <param name="maxThreadCount">Максимальное число параллельных потоков</param>
    public LimitedConcurrencyLevelProcessingStrategy(int maxThreadCount)
    {
      var taskScheduler = new LimitedConcurrencyLevelTaskScheduler(maxThreadCount);
      _taskFactory = new TaskFactory(taskScheduler);
    }

    /// <inheritdoc />
    public void ProcessAsync(Action action, Action<Exception> errorHandler)
    {
      _taskFactory.StartNew(action)
                  .ContinueWith(t => errorHandler(t.Exception),
                                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
    }
  }
}