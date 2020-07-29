using FluentNHibernate.Mapping;
using OrmService.Entities;

namespace OrmService.Mappings
{
  internal class GreetingMap: ClassMap<Greeting>
  {
    public GreetingMap()
    {
      Table("GREETINGS");

      Id(x => x.Id);
      Map(x => x.Value);
    }
  }
}
