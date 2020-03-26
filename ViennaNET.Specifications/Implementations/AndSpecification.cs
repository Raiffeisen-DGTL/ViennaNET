using System;
using System.Linq.Expressions;

namespace ViennaNET.Specifications.Implementations
{
  /// <summary>
  /// Спецификация, соединяющая две спецификации с оператором И
  /// </summary>
  /// <typeparam name="T">Тип объекта спецификации</typeparam>
  public sealed class AndSpecification<T> : Specification<T>
  {
    /// <summary>
    /// Инициализирует экземпляр ссылками на <see cref="Specification{T}"/>
    /// </summary>
    /// <param name="firstSpec">Ссылка на спецификацию</param>
    /// <param name="secondSpec">Ссылка на спецификацию</param>
    public AndSpecification(Specification<T> firstSpec, Specification<T> secondSpec)
    {
      var firstExp = firstSpec.ToExpression();
      var secondExp = secondSpec.ToExpression();

      expression = Expression.Lambda<Func<T, bool>>(Expression.And(firstExp.Body, Expression.Invoke(secondExp, firstExp.Parameters)),
                                                    firstExp.Parameters);
    }

    /// <inheritdoc />
    public override Expression<Func<T, bool>> ToExpression()
    {
      return expression;
    }
  }
}
