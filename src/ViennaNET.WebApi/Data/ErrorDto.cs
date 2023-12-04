namespace ViennaNET.WebApi.Data
{
  public class ErrorDto
  {
    public ErrorDto(string message)
    {
      Message = message;
    }

    public string Message { get; }
  }
}