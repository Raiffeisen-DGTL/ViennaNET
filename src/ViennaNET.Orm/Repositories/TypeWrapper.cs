using System;
using System.Collections.Generic;

namespace ViennaNET.Orm.Repositories
{
  /// <summary>
  ///   Сохраняет значение вместе с типом этого значения
  /// </summary>
  public class TypeWrapper
  {
    public TypeWrapper(object value, Type type)
    {
      BaseValue = value;
      Type = type;
    }

    /// <summary>
    ///   Значение
    /// </summary>
    public object BaseValue { get; }

    /// <summary>
    ///   Тип
    /// </summary>
    public Type Type { get; }

    /// <summary>
    ///   Create TypeWrapper instance with shorthand code like:
    ///   var userDto = new UserDto { Id = 123, Login = "user1Login" };
    ///   .......
    ///   Parameters = new <see cref="Dictionary{TKey,TValue}" /> {
    ///   {"userId", TypeWrapper.Create(userDto, a => a.Id)},
    ///   {"userLogin", TypeWrapper.Create(userDto, a => a.Login)},
    ///   };
    /// </summary>
    /// <param name="dto">A class which contains data</param>
    /// <param name="valueGetter">Lambda expression to get particular field from <paramref name="dto" /></param>
    /// <typeparam name="T">Type of DTO</typeparam>
    /// <typeparam name="V">Type of value</typeparam>
    /// <returns></returns>
    public static TypeWrapper Create<T, V>(T dto, Func<T, V> valueGetter)
    {
      return new(valueGetter(dto), typeof(V));
    }
  }
}