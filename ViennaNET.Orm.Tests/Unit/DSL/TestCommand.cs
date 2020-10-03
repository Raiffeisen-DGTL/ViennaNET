using System.Collections.Generic;
using ViennaNET.Orm.Repositories;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm.Tests.Unit.DSL
{
  public class TestCommand : BaseCommand, IEntityKey<int>
  {
    public TestCommand()
    {
      Parameters = new Dictionary<string, TypeWrapper>
      {
        { "param", new TypeWrapper(12L, typeof(long)) }
      };
    }

    public int Id { get; }
  }
}
