using OrmService.Entities;
using OrmService.Queries;
using ViennaNET.Orm;

namespace OrmService
{
  internal sealed class DbContext : ApplicationContext
  {
    private const string dbConnectionName = "sqlite";

    public DbContext()
    {
      AddEntity<Greeting>(dbConnectionName);
      AddCustomQuery<GreetingsValuesQueryItem>(dbConnectionName);
    }
  }
}
