using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Company.Utils
{
  /// <summary>
  /// Представляет асинхронную отложенную инициализацию
  /// </summary>
  /// <typeparam name="T">Тип результата</typeparam>
  public class AsyncLazy<T> : Lazy<Task<T>>
  {
    /// <summary>
    /// Инициализирует экземпляр ссылкой на функцию, возвращающую задачу
    /// </summary>
    /// <param name="taskFactory"></param>
    public AsyncLazy(Func<Task<T>> taskFactory) : base(() => Task.Factory.StartNew(taskFactory)
                                                                 .Unwrap())
    {
    }

    /// <summary>
    /// Получает awaiter для того, чтобы класс ожно было
    /// использовать с ключевым словом await
    /// </summary>
    /// <returns></returns>
    public TaskAwaiter<T> GetAwaiter()
    {
      return Value.GetAwaiter();
    }
  }
}
