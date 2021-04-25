using FluentNHibernate.Mapping;

namespace ViennaNET.Orm.Tests.LocalDatabaseTests.EntityFactoryTestData
{
  internal class TestEntityMap : ClassMap<TestEntity>
  {
    public TestEntityMap()
    {
      HibernateMapping.Not.AutoImport();

      Table("TestTable");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Status);
    }

  }
}
