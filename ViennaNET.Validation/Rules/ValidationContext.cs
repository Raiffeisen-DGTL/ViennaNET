namespace ViennaNET.Validation.Rules
{
  /// <summary>
  /// Контекст валидации. Позволяет установить любой объект, который
  /// будет вспомогательным для правил валидации. В него можно поместить,
  /// например, кешированные данные из внешних систем для оптимизации работы
  /// </summary>
  public class ValidationContext
  {
    /// <summary>
    /// Инициализирует контекст валидации объектом контекста
    /// </summary>
    /// <param name="context">Объект контекста</param>
    public ValidationContext(object context)
    {
      Context = context;
    }

    /// <summary>
    /// Объект контекста
    /// </summary>
    public object Context { get; set; }
  }
}
