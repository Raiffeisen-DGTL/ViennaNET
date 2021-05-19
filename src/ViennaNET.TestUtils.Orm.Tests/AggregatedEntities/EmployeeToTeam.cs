using System;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.TestUtils.Orm.Tests.AggregatedEntities
{
  public class EmployeeToTeam : IEntityKey<int>
  {
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public Team Team { get; set; } = new Team();

    public int Id { get; }
  }
}