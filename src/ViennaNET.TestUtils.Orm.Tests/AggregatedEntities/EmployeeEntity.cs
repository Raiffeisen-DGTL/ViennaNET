using System.Collections.Generic;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.TestUtils.Orm.Tests.AggregatedEntities
{
  public class EmployeeEntity : IEntityKey<int>
  {
    public string Fio { get; set; }

    public List<EmployeeToTeam> EmployeeInTeamsHistory { get; set; } = new List<EmployeeToTeam>();

    public Team CurrentTeam { get; set; } = new Team();

    public int Id { get; }
  }
}