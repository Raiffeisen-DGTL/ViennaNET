using System.Collections.Generic;
using ViennaNET.Orm.Repositories;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm.Tests.Unit
{
  public class TestCommand : BaseCommand, IEntityKey<int>
  {
    public TestCommand()
    {
      Parameters = new Dictionary<string, TypeWrapper>();
      Parameters.Add("param", new TypeWrapper(12L, typeof(long)));
    }

    public int Id { get; }
  }
}
