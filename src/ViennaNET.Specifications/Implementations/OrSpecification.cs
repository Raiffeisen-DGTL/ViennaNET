using System;
using System.Linq.Expressions;

namespace ViennaNET.Specifications.Implementations
{
  /// <summary>
  ///   Спецификация, соединяющая две спецификации с оператором ИЛИ
  /// </summary>
  /// <typeparam name="T">Тип объекта спецификации</typeparam>
  public sealed class OrSpecification<T> : Specification<T>
  {
    /// <summary>
    ///   Инициализирует экземпляр ссылками на <see cref="Specification{T}" />
    /// </summary>
    /// <param name="firstSpec">Ссылка на спецификацию</param>
    /// <param name="secondSpec">Ссылка на спецификацию</param>
    public OrSpecification(Specification<T> firstSpec, Specification<T> secondSpec)
    {
      var firstExp = firstSpec.ToExpression();
      var secondExp = secondSpec.ToExpression();

      expression = Expression.Lambda<Func<T, bool>>(Expression.Or(firstExp
          .Body, Expression.Invoke(secondExp,
          firstExp
            .Parameters)),
        firstExp
          .Parameters);
    }

    /// <inheritdoc />
    public override Expression<Func<T, bool>> ToExpression()
    {
      return expression;
    }
  }
}