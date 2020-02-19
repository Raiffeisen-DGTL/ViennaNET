using System;

namespace ViennaNET.Validation
{
  /// <inheritdoc />
  /// <summary>
  /// Типизированные данные события
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class EventArgs<T> : EventArgs
  {
    /// <summary>
    /// Инициализирует данные события ссылкой на экземпляр <see cref="T"/>
    /// </summary>
    /// <param name="value">Ссылка на экземпляр <see cref="T"/></param>
    public EventArgs(T value)
    {
      Value = value;
    }

    /// <summary>
    /// Данные события
    /// </summary>
    public T Value { get; }

    public override string ToString()
    {
      return "Value=" + Value;
    }

    public static implicit operator EventArgs<T>(T value)
    {
      return new EventArgs<T>(value);
    }
  }
}
