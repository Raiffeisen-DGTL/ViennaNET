using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm.Tests.LocalDatabaseTests.EntityFactoryTestData
{
  internal class TestEntity : IEntityKey<int>
  {
    public virtual int Id { get; }

    public virtual int Status { get; set; }
  }
}
