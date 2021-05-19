using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Utils.Threading
{
  /// original implementation has been taken from this url https://msdn.microsoft.com/en-us/library/ee789351.aspx
  /// Provides a task scheduler that ensures a maximum concurrency level while 
  /// running on top of the thread pool.
  public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
  {
    // Indicates whether the current thread is processing work items.
    [ThreadStatic]
    private static bool currentThreadIsProcessingItems;

    // The maximum concurrency level allowed by this scheduler. 

    /// <summary>
    ///   mutex
    /// </summary>
    private readonly object _mutex = new object();

    // The list of tasks to be executed 
    private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)

    /// <summary>
    ///   Indicates whether the scheduler is currently processing work items.
    /// </summary>
    private int _delegatesQueuedOrRunning;

    /// <summary>
    ///   Creates a new instance with the specified degree of parallelism.
    /// </summary>
    public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
    {
      if (maxDegreeOfParallelism < 1)
      {
        throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism));
      }

      MaximumConcurrencyLevel = maxDegreeOfParallelism;
    }

    /// <summary>
    ///   Gets the maximum concurrency level supported by this scheduler.
    /// </summary>
    public sealed override int MaximumConcurrencyLevel { get; }

    /// <summary>
    ///   Queues a task to the scheduler.
    /// </summary>
    protected sealed override void QueueTask(Task task)
    {
      // Add the task to the list of tasks to be processed.  If there aren't enough 
      // delegates currently queued or running to process tasks, schedule another. 
      lock (_mutex)
      {
        _tasks.AddLast(task);
        if (_delegatesQueuedOrRunning < MaximumConcurrencyLevel)
        {
          ++_delegatesQueuedOrRunning;
          NotifyThreadPoolOfPendingWork();
        }
      }
    }

    // Inform the ThreadPool that there's work to be executed for this scheduler. 
    private void NotifyThreadPoolOfPendingWork()
    {
      ThreadPool.UnsafeQueueUserWorkItem(_ =>
      {
        // Note that the current thread is now processing work items.
        // This is necessary to enable inlining of tasks into this thread.
        currentThreadIsProcessingItems = true;
        try
        {
          // Process all available items in the queue.
          while (true)
          {
            Task item;
            lock (_mutex)
            {
              // When there are no more items to be processed,
              // note that we're done processing, and get out.
              if (_tasks.Count == 0)
              {
                --_delegatesQueuedOrRunning;
                break;
              }

              // Get the next item from the queue
              item = _tasks.First.Value;
              _tasks.RemoveFirst();
            }

            // Execute the task we pulled out of the queue
            TryExecuteTask(item);
          }
        }
        // We're done processing items on the current thread
        finally
        {
          currentThreadIsProcessingItems = false;
        }
      }, null);
    }

    /// <summary>
    ///   Attempts to execute the specified task on the current thread.
    /// </summary>
    protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
      // If this thread isn't already processing a task, we don't support inlining
      if (!currentThreadIsProcessingItems)
      {
        return false;
      }

      // If the task was previously queued, remove it from the queue
      if (!taskWasPreviouslyQueued)
      {
        return TryExecuteTask(task);
      }

      // Try to run the task. 
      return TryDequeue(task) && TryExecuteTask(task);
    }

    /// <summary>
    ///   Attempt to remove a previously scheduled task from the scheduler.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    protected sealed override bool TryDequeue(Task task)
    {
      lock (_mutex)
      {
        return _tasks.Remove(task);
      }
    }

    /// <summary>
    ///   Gets an enumerable of the tasks currently scheduled on this scheduler.
    /// </summary>
    /// <returns></returns>
    protected sealed override IEnumerable<Task> GetScheduledTasks()
    {
      lock (_mutex)
      {
        return _tasks.ToArray();
      }
    }
  }
}