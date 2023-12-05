using System;
using System.Linq.Expressions;

namespace ViennaNET.Specifications.Implementations
{
  /// <summary>
  ///   Спецификация, применяющая к другой спецификации оператор НЕ
  /// </summary>
  /// <typeparam name="T">Тип объекта спецификации</typeparam>
  public sealed class NotSpecification<T> : Specification<T>
  {
    /// <summary>
    ///   Инициализирует экземпляр ссылкой на <see cref="Specification{T}" />
    /// </summary>
    /// <param name="spec">Ссылка на спецификацию</param>
    public NotSpecification(Specification<T> spec)
    {
      var exp = spec.ToExpression();

      expression = Expression.Lambda<Func<T, bool>>(Expression.Not(exp.Body), exp.Parameters);
    }

    /// <inheritdoc />
    public override Expression<Func<T, bool>> ToExpression()
    {
      return expression;
    }
  }
}