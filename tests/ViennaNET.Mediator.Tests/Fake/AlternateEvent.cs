using System;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator.Tests.Fake
{
  public class AlternateEvent : IEvent
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public AggregateException Exception { get; set; }
  }
}