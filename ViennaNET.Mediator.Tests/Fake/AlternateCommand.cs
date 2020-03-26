namespace ViennaNET.Mediator.Tests.Fake
{
  public class AlternateCommand : ICommand
  {
    public AlternateCommand()
    {
      IsCompleted = false;
      Reason = "";
    }

    public string Name { get; set; }

    public bool IsCompleted { get; set; }

    public object Reason { get; set; }
  }
}
