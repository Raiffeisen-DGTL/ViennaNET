using System.Collections.Generic;
using ViennaNET.Orm.Repositories;

namespace ViennaNET.Orm.Tests.Unit.DSL
{
  public class TestQuery : BaseQuery<object>
  {
    public TestQuery()
    {
      Parameters = new Dictionary<string, object> { { "param", 12L } };
    }

    protected override object TransformTuple(object[] tuple, string[] aliases)
    {
      return new();
    }
  }
}