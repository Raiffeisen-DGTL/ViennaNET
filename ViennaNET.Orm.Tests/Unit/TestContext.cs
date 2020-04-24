namespace ViennaNET.Orm.Tests.Unit
{
  internal class TestContext : ApplicationContext
  {
    public TestContext(bool registerValueObject = false, bool useDefaultNick = false, bool useEntityWithoutKey = false)
    {
      if (useDefaultNick)
      {
        AddEntity<TestCommand>();
        AddCommand<TestCommand>();
        AddNamedQuery<TestQuery>("testQuery");
      }
      else
      {
        AddEntity<TestCommand>("mssql");
        AddCommand<TestCommand>("db2");
        AddNamedQuery<TestQuery>("testQuery", "oracle");
      }

      if (useEntityWithoutKey)
      {
        AddEntity<BadEntity>();
      }

      if (registerValueObject)
      {
        AddCustomQuery<TestCommand>("oracle");
      }
    }
  }
}
