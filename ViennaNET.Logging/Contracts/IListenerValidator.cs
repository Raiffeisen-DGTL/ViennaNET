using ViennaNET.Logging.Configuration;

namespace ViennaNET.Logging.Contracts
{
  public interface IListenerValidator
  {
    void Validate(LogListener listener);
  }
}