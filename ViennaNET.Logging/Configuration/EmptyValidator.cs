using ViennaNET.Logging.Contracts;

namespace ViennaNET.Logging.Configuration
{
  internal sealed class EmptyValidator : IListenerValidator
  {
    public void Validate(LogListener listener)
    {
    }
  }
}
