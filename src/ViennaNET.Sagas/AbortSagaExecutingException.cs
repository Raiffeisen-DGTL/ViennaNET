using System;

namespace ViennaNET.Sagas
{
  /// <summary>
  /// Исключение для инициализации процесса отката выполнения саги
  /// </summary>
  public class AbortSagaExecutingException : Exception
  {
  }
}
