using Polly;

namespace ViennaNET.ArcSight
{
  /// <summary>
  /// Фабрика для создания политики обработки ошибок в случае исключения от ArcSight
  /// </summary>
  public interface IErrorHandlingPoliciesFactory
  {
    /// <summary>
    /// Создает экземпляр политики для автоматического повторного
    /// вызова метода в случае определенного исключения. 
    /// </summary>
    /// <returns>Экземпляр политики</returns>
    ISyncPolicy CreateStdCommunicationPolicy();
  }
}