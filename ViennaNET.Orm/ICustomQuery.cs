using NHibernate.Transform;

namespace ViennaNET.Orm
{
  /// <summary>
  /// Интерфейс-маркер запроса к БД. Запрос должен содержать
  /// SQL-скрипт для выполнения
  /// </summary>
  public interface ICustomQuery
  {
    /// <summary>
    /// SQL-скрипт для выполнения
    /// </summary>
    string Sql { get; }

    /// <summary>
    /// Трансформатор результата выполнения запроса
    /// </summary>
    IResultTransformer Transformer { get; }
  }
}
