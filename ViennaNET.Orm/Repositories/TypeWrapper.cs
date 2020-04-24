using System;

namespace ViennaNET.Orm.Repositories
{
  /// <summary>
  /// Сохраняет значение вместе с типом этого значения
  /// </summary>
  public class TypeWrapper
  {
    public TypeWrapper(object value, Type type)
    {
      BaseValue = value;
      Type = type;
    }

    /// <summary>
    /// Значение
    /// </summary>
    public object BaseValue { get; }

    /// <summary>
    /// Тип
    /// </summary>
    public Type Type { get; }
  }
}