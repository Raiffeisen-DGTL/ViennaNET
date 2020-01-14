using Company.Logging.Contracts;

namespace Company.Logging.Configuration
{
  internal sealed class EmptyValidator : IListenerValidator
  {
    public void Validate(LogListener listener)
    {
    }
  }
}
