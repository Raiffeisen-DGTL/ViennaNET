namespace Company.Logging.Contracts
{
  public interface ILogConfiguration
  {
    ILog BuildLogger();    
  }
}