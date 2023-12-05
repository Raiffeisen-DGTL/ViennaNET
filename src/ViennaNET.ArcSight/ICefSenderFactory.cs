using ViennaNET.ArcSight.Configuration;
using ViennaNET.ArcSight.Exceptions;

namespace ViennaNET.ArcSight
{
  /// <summary>
  ///   Фабрика создает <see cref="CefSender" /> на основе данных конфигурации
  /// </summary>
  public interface ICefSenderFactory
  {
    /// <summary>
    ///   Метод создает экземпляр отправителя сообщений
    ///   на основе данных из конфигурации
    /// </summary>
    /// <param name="cefConfig">Конфигурационные параметры</param>
    /// <returns>Экземпляр отправителя сообщений</returns>
    /// <exception cref="ArcSightConfigurationException">
    ///   Возникает при недопустимом значении параметра Protocol
    ///   из конфигурации. Допустимые значения - tcp, udp, local
    /// </exception>
    ICefSender CreateSender(ArcSightSection cefConfig);
  }
}