using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ViennaNET.TestUtils.Orm.NhQueryable
{
  internal class NhQueryableProxy<T> : IOrderedQueryable<T>
  {
    public NhQueryableProxy(ICollection<T> items)
    {
      Provider = new NhQueryProviderProxy<T>(items);
      Expression = Expression.Constant(this);
    }

    public NhQueryableProxy(IQueryProvider provider, Expression expression)
    {
      Provider = provider;
      Expression = expression;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return Provider.Execute<IEnumerable<T>>(Expression)
        .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public Type ElementType { get; } = typeof(T);

    public Expression Expression { get; }

    public IQueryProvider Provider { get; }
  }
}