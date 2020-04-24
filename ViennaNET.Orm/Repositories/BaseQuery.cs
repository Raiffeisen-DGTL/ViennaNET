using System.Collections;
using System.Collections.Generic;
using ViennaNET.Utils;
using NHibernate.Transform;

namespace ViennaNET.Orm.Repositories
{
  /// <inheritdoc />
  public abstract class BaseQuery<T> : ICustomQuery
  {
    protected string Sql;

    /// <summary>
    /// Параметры для записи в формируемый SQL-запрос
    /// </summary>
    public IDictionary<string, object> Parameters { get; set; }

    /// <inheritdoc />
    string ICustomQuery.Sql => Sql;

    /// <inheritdoc />
    IResultTransformer ICustomQuery.Transformer => new TransformerImpl(this);

    protected virtual IList TransformList(IList collection)
    {
      return collection;
    }

    protected abstract T TransformTuple(object[] tuple, string[] aliases);

    private class TransformerImpl : IResultTransformer
    {
      private readonly BaseQuery<T> _query;

      public TransformerImpl(BaseQuery<T> query)
      {
        _query = query.ThrowIfNull(nameof(query));
      }

      public object TransformTuple(object[] tuple, string[] aliases)
      {
        return _query.TransformTuple(tuple, aliases);
      }

      public IList TransformList(IList collection)
      {
        return _query.TransformList(collection);
      }
    }
  }
}
