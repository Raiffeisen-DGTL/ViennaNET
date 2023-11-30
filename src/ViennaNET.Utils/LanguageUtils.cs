using System.Linq;

namespace ViennaNET.Utils
{
  /// <summary>
  /// Вспомогательные методы расширения языка
  /// </summary>
  public static class LanguageUtils
  {
    /// <summary>
    /// Проверяет объект на принадлежность к массиву
    /// </summary>
    /// <typeparam name="T">Тип проверяемого объекта</typeparam>
    /// <param name="instance">Проверяемый объект</param>
    /// <param name="expected">Массив объектов</param>
    /// <returns>Результат проверки вхождения объекта в массив</returns>
    public static bool In<T>(this T instance, params T[] expected)
    {
      return instance != null && expected.Any(x => instance.Equals(x));
    }
  }
}