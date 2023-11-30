﻿namespace ViennaNET.Mediator.Tests.Fake
{
  public class Command : ICommand
  {
    public Command()
    {
      IsCompleted = false;
      Reason = "";
    }

    public string Name { get; set; }

    public bool IsCompleted { get; set; }

    public object Reason { get; set; }
  }
}