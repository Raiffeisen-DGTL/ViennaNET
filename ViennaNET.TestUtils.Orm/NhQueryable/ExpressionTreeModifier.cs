using System.Linq;
using System.Linq.Expressions;

namespace ViennaNET.TestUtils.Orm.NhQueryable
{
  internal class ExpressionTreeModifier<T> : ExpressionVisitor
  {
    private static readonly string[] nHibernateLinqMethods = {"FetchMany", "Fetch", "ThenFetch", "ThenFetchMany"};

    private readonly IQueryable<T> _queryableData;

    internal ExpressionTreeModifier(IQueryable<T> queryableData)
    {
      _queryableData = queryableData;
    }

    protected override Expression VisitConstant(ConstantExpression c)
    {
      // Here the magic happens: the expression types are all NHibernateQueryableProxy,
      // so we replace them by the correct ones
      if (c.Type == typeof(NhQueryableProxy<T>))
      {
        return Expression.Constant(_queryableData);
      }

      return c;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
      var methodName = node.Method.Name;
      if (nHibernateLinqMethods.Contains(methodName))
      {
        return base.Visit(node.Arguments[0]);
      }

      return base.VisitMethodCall(node);
    }
  }
}