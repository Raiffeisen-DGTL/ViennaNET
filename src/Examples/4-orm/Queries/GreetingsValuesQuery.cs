using System.Collections.Generic;
using ViennaNET.Orm.Repositories;

namespace OrmService.Queries
{
  public class GreetingsValuesQuery : BaseQuery<GreetingsValuesQueryItem>
  {
    private const string sql = "SELECT VALUE FROM GREETINGS";
    private const string condition = " WHERE ID = :id";

    public GreetingsValuesQuery(int? id)
    {
      var resultSql = sql;

      if (id.HasValue)
      {
        resultSql += condition;

        Parameters = new Dictionary<string, object>
        {
          { "id", id }
        };
      }

      Sql = resultSql;
    }

    protected override GreetingsValuesQueryItem TransformTuple(object[] tuple, string[] aliases)
    {
      return GreetingsValuesQueryItem.Create(value: (string)tuple[0]);
    }
  }
}
