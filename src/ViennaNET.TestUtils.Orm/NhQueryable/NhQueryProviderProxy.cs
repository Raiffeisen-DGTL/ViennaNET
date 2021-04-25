using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Type;

namespace ViennaNET.TestUtils.Orm.NhQueryable
{
  internal class NhQueryProviderProxy<T> : INhQueryProvider
  {
    private readonly ICollection<T> _data;

    public NhQueryProviderProxy(ICollection<T> data)
    {
      _data = data;
    }

    public IQueryable CreateQuery(Expression expression)
    {
      var elementType = TypeSystemHelper.GetElementType(expression.Type);

      return (IQueryable)Activator.CreateInstance(typeof(NhQueryableProxy<>)
        .MakeGenericType(elementType), this, expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
      var newExpression = new ExpressionTreeModifier<T>(_data.AsQueryable()).Visit(expression);
      return new NhQueryableProxy<TElement>(this, newExpression);
    }

    public object Execute(Expression expression)
    {
      return _data.AsQueryable().Provider.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
      var newExpression = new ExpressionTreeModifier<T>(_data.AsQueryable()).Visit(expression);
      if (typeof(TResult).Name == typeof(IEnumerable<>).Name)
      {
        return (TResult)_data.AsQueryable().Provider.CreateQuery(newExpression);
      }

      return _data.AsQueryable().Provider.Execute<TResult>(newExpression);
    }


    public Task<int> ExecuteDmlAsync<T1>(QueryMode queryMode, Expression expression,
      CancellationToken cancellationToken)
    {
      return Task.FromResult(ExecuteDml<T1>(queryMode, expression));
    }

    public IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression)
    {
      throw new NotImplementedException();
    }

    public IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression)
    {
      throw new NotImplementedException();
    }

    public void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression,
      IDictionary<string, Tuple<object, IType>> parameters)
    {
      throw new NotImplementedException();
    }

    public int ExecuteDml<T1>(QueryMode queryMode, Expression expression)
    {
      if (queryMode == QueryMode.Delete)
      {
        var itemsToDelete = Execute<IEnumerable<T>>(expression).ToList();
        foreach (var itemToDelete in itemsToDelete)
        {
          _data.Remove(itemToDelete);
        }

        return itemsToDelete.Count;
      }

      throw new NotImplementedException();
    }

    public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
      return Task.FromResult(Execute<TResult>(expression));
    }
  }
}