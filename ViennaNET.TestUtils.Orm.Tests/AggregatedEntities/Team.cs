using System.Collections.Generic;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.TestUtils.Orm.Tests.AggregatedEntities
{
  public class Team : IEntityKey<int>
  {
    public List<EmployeeEntity> Managers { get; set; } = new List<EmployeeEntity>();

    public TeamCostCenter TeamCostCenter { get; set; }

    public int Id { get; }
  }
}