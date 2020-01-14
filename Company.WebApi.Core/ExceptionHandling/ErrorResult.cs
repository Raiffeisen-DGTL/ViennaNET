namespace Company.WebApi.Core.ExceptionHandling
{
  public class ErrorResult
  {
    public string Message { get; set; }
    public string ExceptionType { get; set; }
    public string StackTrace { get; set; }
  }
}
