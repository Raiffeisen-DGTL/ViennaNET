namespace ViennaNET.Orm.Tests.LocalDatabaseTests.EntityFactoryTestData
{
  internal class TestDataContext : ApplicationContext
  {
    public TestDataContext()
    {
      AddEntity<TestEntity>();
    }
  }
}
