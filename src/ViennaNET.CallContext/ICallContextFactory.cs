namespace ViennaNET.CallContext
{
  /// <summary>
  /// Класс для извлечения актуального контекста вызова
  /// </summary>
  public interface ICallContextFactory
  {
    /// <summary>
    /// Возвращает актуальный контекст вызова
    /// </summary>
    /// <returns></returns>
    ICallContext Create();
  }
}
