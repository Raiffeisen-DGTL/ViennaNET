namespace ViennaNET.ArcSight
{
  /// <summary>
  ///   Интерфейс отправки сообщения в ArcSight.
  /// </summary>
  public interface IArcSightClient
  {
    /// <summary>
    ///   Сериализует и отправляет сообщение в ArcSight
    /// </summary>
    /// <param name="message">Сообщение в стандартном CEF-формате</param>
    void Send(CefMessage message);
  }
}