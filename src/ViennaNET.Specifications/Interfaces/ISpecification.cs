namespace ViennaNET.Specifications.Interfaces
{
  /// <summary>
  ///   Интерфейс спецификации
  /// </summary>
  /// <typeparam name="T">Тип объекта для спецификации</typeparam>
  internal interface ISpecification<in T>
  {
    /// <summary>
    ///   Определяет, удовлетворяет ли объект спецификации
    /// </summary>
    /// <param name="entity">Объект для проверки</param>
    /// <returns>Признак соответствия спецификации</returns>
    bool IsSatisfiedBy(T entity);
  }
}