namespace MessagingService.Services
{
  public class EnglishService: IEnglishService
  {
    public string Greet()
    {
      return "Hello";
    }

    public string Farewell()
    {
      return "Good bye";
    }
  }
}
