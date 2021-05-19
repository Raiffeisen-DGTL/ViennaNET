namespace OrmService.Queries
{
  public class GreetingsValuesQueryItem
  {
    public string Value { get; private set; }

    public static GreetingsValuesQueryItem Create(string value)
    {
      return new GreetingsValuesQueryItem()
      {
        Value = value
      };
    }
  }
}
