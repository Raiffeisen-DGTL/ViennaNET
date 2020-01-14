using Company.Logging.Configuration;

namespace Company.Logging.Contracts
{
  public interface IListenerValidator
  {
    void Validate(LogListener listener);
  }
}