using System;

namespace ViennaNET.ArcSight.Exceptions
{
  /// <summary>
  ///   Исключение, возникающее при наличии ошибки в конфигурации
  /// </summary>
  public class ArcSightConfigurationException : Exception
  {
    public ArcSightConfigurationException(string message) : base(message)
    {
    }
  }
}