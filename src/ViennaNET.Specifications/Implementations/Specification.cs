using System;
using System.Linq.Expressions;
using ViennaNET.Specifications.Interfaces;

namespace ViennaNET.Specifications.Implementations
{
  /// <summary>
  ///   Базовый класс спецификации. Позволяет преобразовать
  ///   спецификацию в элемент дерева выражений
  /// </summary>
  /// <typeparam name="T">Тип объекта спецификации</typeparam>
  public abstract class Specification<T> : ISpecification<T>
  {
    protected Expression<Func<T, bool>> expression = null;

    /// <inheritdoc />
    public bool IsSatisfiedBy(T entity)
    {
      var predicate = ToExpression().Compile();
      return predicate(entity);
    }

    /// <summary>
    ///   Возвращает выражение <see cref="Expression" /> для спецификации
    /// </summary>
    /// <returns>Выражение <see cref="Expression" /></returns>
    public abstract Expression<Func<T, bool>> ToExpression();

    /// <summary>
    ///   Соединяет две спецификации с оператором И
    /// </summary>
    /// <param name="spec1">Ссылка на спецификацию</param>
    /// <param name="spec2">Ссылка на спецификацию</param>
    /// <returns>Объединенная спецификация</returns>
    public static AndSpecification<T> operator &(Specification<T> spec1, Specification<T> spec2)
    {
      return new(spec1, spec2);
    }

    /// <summary>
    ///   Соединяет две спецификации с оператором ИЛИ
    /// </summary>
    /// <param name="spec1">Ссылка на спецификацию</param>
    /// <param name="spec2">Ссылка на спецификацию</param>
    /// <returns>Объединенная спецификация</returns>
    public static OrSpecification<T> operator |(Specification<T> spec1, Specification<T> spec2)
    {
      return new(spec1, spec2);
    }

    /// <summary>
    ///   Применяет к переданной спецификации оператор НЕ
    /// </summary>
    /// <param name="spec">Ссылка на спецификацию</param>
    /// <returns>Спецификация с применением оператора НЕ</returns>
    public static NotSpecification<T> operator !(Specification<T> spec)
    {
      return new(spec);
    }
  }
}